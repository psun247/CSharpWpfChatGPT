using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Speech.Synthesis;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Whetstone.ChatGPT;
using Whetstone.ChatGPT.Models;
using CSharpWpfChatGPT.Models;
using CSharpWpfChatGPT.Services;
using CSharpWpfChatGPT.Helpers;

namespace CSharpWpfChatGPT.LiveChat
{
    // C# .NET 6 / 8 WPF, Whetstone ChatGPT, CommunityToolkit MVVM, ModernWpfUI, RestoreWindowPlace
    public partial class LiveChatViewModel : ObservableObject
    {
        private IHistoryRepo _historyRepo;
        private WhetstoneChatGPTService _chatGPTService;        
        private LiveChatManager _liveChatManager = new LiveChatManager();
        private List<string> _chatInputList = new List<string>();
        private int _chatInputListIndex = -1;

        public LiveChatViewModel(IHistoryRepo historyRepo, WhetstoneChatGPTService chatGPTService)
        {            
            _historyRepo = historyRepo;
            _chatGPTService = chatGPTService;
            SelectedChat = _liveChatManager.AddNewChat();
            ChatList = new ObservableCollection<Chat>(_liveChatManager.ChatList);
            ChatInput = "Please list top 5 ChatGPT prompts";

            // Uncomment this to insert testing data
            // DevDebugInitialize();
        }

        public Action<UpdateUIEnum>? UpdateUIAction { get; set; }
        public bool IsCommandNotBusy => !IsCommandBusy;
        [ObservableProperty]
        private bool _isCommandBusy;        
        public ObservableCollection<Chat> ChatList { get; }
        [ObservableProperty]
        private Chat _selectedChat;
        [ObservableProperty]
        private string _chatInput;
        [ObservableProperty]
        private Visibility _imagePaneVisibility = Visibility.Collapsed;
        [ObservableProperty]
        private string _imageInput = "A tennis court";
        [ObservableProperty]
        public byte[]? _resultImage;
        [ObservableProperty]
        public bool _addToHistoryButtonEnabled;
        [ObservableProperty]
        private bool _isStreamingMode = true;
        public string[] LangList { get; } = { "English", "Chinese", "Hindi", "Spanish" };
        [ObservableProperty]
        public string _selectedLang = "Spanish";
        [ObservableProperty]
        bool _isFemaleVoice = true;
        [ObservableProperty]
        string _statusMessage = "Ctrl+Enter for input of multiple lines. Enter-Key to send. Ctrl+UpArrow | DownArrow to navigate previous input lines";

        // Also RelayCommand from AppBar
        [RelayCommand]
        private void NewChat()
        {
            try
            {
                if (!AddNewChatIfNotExists())
                {
                    StatusMessage = "'New Chat' already exists";
                    return;
                }

                UpdateUIAction?.Invoke(UpdateUIEnum.SetFocusToChatInput);

                StatusMessage = "'New Chat' has been added and selected";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }
        
        private void DevDebugInitialize()
        {
            string prompt = "TestPrompt1";
            string promptDisplay = prompt;
            var newMessage = new Message("Me", promptDisplay);
            SelectedChat.AddMessage(newMessage);
            string result = "TestPrompt1 result";
            SelectedChat.AddMessage("Bot", result.Replace("Bot: ", string.Empty));
            PostProcessOnSend(prompt);

            Chat newChat = _liveChatManager.AddNewChat();
            ChatList.Add(newChat);
            SelectedChat = newChat;
            prompt = "TestPrompt2";
            promptDisplay = prompt;
            newMessage = new Message("Me", promptDisplay);
            SelectedChat.AddMessage(newMessage);
            result = "TestPrompt2 result";
            SelectedChat.AddMessage("Bot", result.Replace("Bot: ", string.Empty));
            PostProcessOnSend(prompt);
        }

        private bool AddNewChatIfNotExists()
        {
            if (_liveChatManager.NewChatExists)
            {
                return false;
            }

            // Note: 'New Chat' will be renamed after it's used
            Chat newChat = _liveChatManager.AddNewChat();
            ChatList.Add(newChat);
            SelectedChat = newChat;
            return true;
        }

        // Up/Previous or down/next chat input in the chat input list
        public void PrevNextChatInput(bool isUp)
        {
            if (_chatInputList.IsNotEmpty())
            {
                if (ChatInput.IsBlank())
                {
                    // Pick the just used last one
                    ChatInput = _chatInputList[_chatInputList.Count - 1];
                    _chatInputListIndex = _chatInputList.Count - 1;
                }
                else
                {
                    if (isUp)
                    {
                        if (_chatInputListIndex <= 0)
                        {
                            _chatInputListIndex = _chatInputList.Count - 1;
                        }
                        else
                        {
                            _chatInputListIndex--;
                        }
                    }
                    else
                    {
                        if (_chatInputListIndex >= _chatInputList.Count - 1)
                        {
                            _chatInputListIndex = 0;
                        }
                        else
                        {
                            _chatInputListIndex++;
                        }
                    }
                }

                // Bind ChatInput
                if (!ChatInput.Equals(_chatInputList[_chatInputListIndex]))
                {
                    ChatInput = _chatInputList[_chatInputListIndex];
                }
            }
        }

        [RelayCommand]
        private void AddToHistory(Chat chat)
        {
            _historyRepo.AddChat(chat);
            FixupChatId(chat);

            // Add chat to HistoryViewModel
            WeakReferenceMessenger.Default.Send(new AddChatMessage(chat));            

            StatusMessage = $"'{chat.Name}' (PK: {chat.Id}) added to History tab";
        }

        // If DB is configured, chat.Id will be PK (i.e. DB insert already called)    
        private void FixupChatId(Chat chat)
        {
            if (chat.Id == 0)
            {
                // DB not configured, assign a max + 1
                chat.Id = ChatList.Count == 0 ? 1 : ChatList.Max(x => x.Id) + 1;
            }
        }

        // Both XAML (important DataContext in DataContext.CopyMessageCommand) binding and menu item
        [RelayCommand]
        public void CopyMessage(Message message)
        {
            int meIndex = SelectedChat.MessageList.IndexOf(message) - 1;
            if (meIndex >= 0)
            {
                Clipboard.SetText($"Me: {SelectedChat.MessageList[meIndex].Text}\n\n{message.Text}");
                StatusMessage = "Both Me and Bot messages copied to clipboard";
            }
            else
            {
                Clipboard.SetText($"Me: {message.Text}");
                StatusMessage = "Me message copied to clipboard";
            }
        }

        [RelayCommand]
        private async Task Send()
        {
            if (IsCommandBusy)
            {
                return;
            }

            if (!ValidateInput(ChatInput, out string prompt))
            {
                return;
            }

            try
            {
                SetCommandBusy(true, isSendCommand: true);

                string previousPrompts = BuildPreviousPrompts();
                if (!string.IsNullOrEmpty(previousPrompts))
                {
                    await Send($"{previousPrompts}\nMe: {prompt}", prompt);
                }
                else
                {
                    await Send(prompt, prompt);
                }

                PostProcessOnSend(prompt);

                StatusMessage = "Ready";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }

            SetCommandBusy(false, isSendCommand: true);

            // Always set focus to ChatInput after Send()
            UpdateUIAction?.Invoke(UpdateUIEnum.SetFocusToChatInput);

            // Always ScrollToBottom
            UpdateUIAction?.Invoke(UpdateUIEnum.MessageListViewScrollToBottom);
        }

        [RelayCommand]
        private async Task Explain()
        {
            await ExecutePost("Explain");
        }

        [RelayCommand]
        private async Task TranslateTo()
        {
            await ExecutePost($"Translate to {SelectedLang}");
        }

        private async Task ExecutePost(string prefix)
        {
            if (IsCommandBusy)
            {
                return;
            }

            if (!ValidateInput(ChatInput, out string prompt))
            {
                return;
            }

            try
            {
                SetCommandBusy(true);

                // 'Explain' or 'Translate to' always uses a new chat
                AddNewChatIfNotExists();

                prompt = $"{prefix} '{prompt}'";
                await Send(prompt, prompt);

                PostProcessOnSend(prompt);

                // Ensure this is marked for logic in BuildPreviousPrompts()
                SelectedChat.IsSend = false;

                StatusMessage = "Ready";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }

            SetCommandBusy(false);

            // Always set focus to ChatInput after Send()
            UpdateUIAction?.Invoke(UpdateUIEnum.SetFocusToChatInput);

            // Always ScrollToBottom
            UpdateUIAction?.Invoke(UpdateUIEnum.MessageListViewScrollToBottom);
        }

        [RelayCommand]
        private void Speak()
        {
            try
            {
                SetCommandBusy(true);

                // Note: need to have voices installedL: Win-Key + I, Time & language -> Speech
                var synthesizer = new SpeechSynthesizer()
                {
                    Volume = 100,  // 0...100
                    Rate = -2,     // -10...10                    
                };

                synthesizer.SelectVoiceByHints(IsFemaleVoice ? VoiceGender.Female : VoiceGender.Male, VoiceAge.Adult);                

                // Asynchronous / Synchronous
                synthesizer.SpeakAsync(ChatInput);
                //synthesizer.Speak(ChatInput);

                StatusMessage = "Done";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }

            SetCommandBusy(false);

            // Always set focus to ChatInput
            UpdateUIAction?.Invoke(UpdateUIEnum.SetFocusToChatInput);
        }

        private bool ValidateInput(string input, out string prompt)
        {
            prompt = input.Trim();
            if (prompt.Length < 2)
            {
                StatusMessage = "Prompt must be at least 2 characters";
                return false;
            }
            return true;
        }

        // Build 'context' for ChatGPT
        private string BuildPreviousPrompts()
        {
            string previousPrompts = string.Empty;
            if (!SelectedChat.IsSend)
            {
                // We are on 'Explain' or 'Translate to', so auto-create a new chat
                AddNewChatIfNotExists();
            }
            else if (SelectedChat.MessageList.IsNotEmpty())
            {
                // Continue with previous chat by sending previousPrompts
                foreach (Message message in SelectedChat.MessageList)
                {
                    previousPrompts += $"{message.Sender}: {message.Text}";
                }
            }
            return previousPrompts;
        }

        private async Task Send(string prompt, string promptDisplay)
        {
            var newMessage = new Message("Me", promptDisplay);
            SelectedChat.AddMessage(newMessage);

            StatusMessage = "Talking to ChatGPT API...please wait";
            if (IsStreamingMode)
            {
                await SendStreamingMode(prompt);
            }
            else
            {
                string result = await DoSend(prompt);
                SelectedChat.AddMessage("Bot", result.Replace("Bot: ", string.Empty));
            }

            // Clear the ChatInput field
            ChatInput = string.Empty;
        }

        private async Task<string> DoSend(string prompt)
        {
            // GPT-3.5
            ChatGPTChatCompletionResponse? completionResponse = await _chatGPTService.CreateChatCompletionAsync(prompt);
            ChatGPTChatCompletionMessage? message = completionResponse?.GetMessage();
            return message?.Content ?? string.Empty;
        }

        private async Task SendStreamingMode(string prompt)
        {
            // Append with message.Text below
            Message message = SelectedChat.AddMessage("Bot", string.Empty);

            // GPT-3.5
            await foreach (ChatGPTChatCompletionStreamResponse? response in
                                _chatGPTService.StreamChatCompletionAsync(prompt).ConfigureAwait(false))
            {
                if (response is not null)
                {
                    string? responseText = response.GetCompletionText();
                    message.Text = message.Text + responseText;
                }
            }
        }

        private void PostProcessOnSend(string prompt)
        {
            // Handle new chat
            if (_liveChatManager.IsNewChat(SelectedChat.Name))
            {
                // After this call, SelectedChat.Name updated on the left panel because SelectedChat is/wraps the new chat                
                _liveChatManager.RenameNewChat(prompt);

                UpdateAddToHistoryButton(SelectedChat);
            }

            // Handle chat input list
            if (!_chatInputList.Any(x => x.Equals(prompt)))
            {
                _chatInputList.Add(prompt);
                _chatInputListIndex = _chatInputList.Count - 1;
            }

            // Handle user input list
            if (!_chatInputList.Any(x => x.Equals(prompt)))
            {
                _chatInputList.Add(prompt);
                _chatInputListIndex = _chatInputList.Count - 1;
            }
        }

        [RelayCommand]
        private void ExpandOrCollapseImagePane()
        {
            ImagePaneVisibility = (ImagePaneVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        [RelayCommand]
        private async Task CreateImage()
        {
            if (!ValidateInput(ImageInput, out string prompt))
            {
                return;
            }

            try
            {
                SetCommandBusy(true);
                StatusMessage = "Creating an image...please wait";

                // Will reject query of an image of a real person
                ResultImage = await _chatGPTService.CreateImageAsync(prompt);

                StatusMessage = $"Processed image request for '{prompt}'";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }

            SetCommandBusy(false);
        }

        // ESC key maps to ClearChatInputCommand
        [RelayCommand]
        private void ClearChatInput()
        {
            ChatInput = string.Empty;
        }

        // ESC key maps to ClearImageInputCommand
        [RelayCommand]
        private void ClearImageInput()
        {
            ImageInput = string.Empty;
        }

        private void SetCommandBusy(bool isCommandBusy, bool isSendCommand = false)
        {
            IsCommandBusy = isCommandBusy;
            OnPropertyChanged(nameof(IsCommandNotBusy));

            if (!isSendCommand)
            {
                Mouse.OverrideCursor = IsCommandBusy ? Cursors.Wait : null;
            }
        }

        // partial method (CommunityToolkit MVVM)
        partial void OnSelectedChatChanged(Chat value)
        {
            UpdateAddToHistoryButton(value);

            if (value != null)
            {
                // Re-setup on selected chat changed
                UpdateUIAction?.Invoke(UpdateUIEnum.SetupMessageListViewScrollViewer);                
            }            
        }

        private void UpdateAddToHistoryButton(Chat value)
        {
            AddToHistoryButtonEnabled = value != null && !_liveChatManager.IsNewChat(value.Name);
        }
    }
}
