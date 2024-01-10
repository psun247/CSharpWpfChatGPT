using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Whetstone.ChatGPT;
using Whetstone.ChatGPT.Models;
using CSharpWpfChatGPT.Models;
using CSharpWpfChatGPT.Services;
using CSharpWpfChatGPT.Helpers;

namespace CSharpWpfChatGPT
{
    // C# .NET 6 / 8 WPF, Whetstone ChatGPT, CommunityToolkit MVVM, ModernWpfUI, RestoreWindowPlace
    public partial class ChatViewModel : ObservableObject
    {        
        private WhetstoneChatGPTService _chatGPTService;
        private ChatHistory _chatHistory;
        private List<string> _chatInputList;
        private int _chatInputListIndex;

        public ChatViewModel(WhetstoneChatGPTService chatGPTService)
        {
            _chatGPTService = chatGPTService;
            _chatHistory = new ChatHistory();
            _chatHistory.AddChat("New Chat");
            ChatList = new ObservableCollection<Chat>(_chatHistory.ChatList);
            _selectedChat = ChatList[0];
            _chatInputList = new List<string>();
            _chatInputListIndex = -1;
            _chatInput = "Top .NET features as of 2024";

            // <Version>1.1</Version> in .csproj
            Version appVer = Assembly.GetExecutingAssembly().GetName().Version!;
            Version dotnetVer = Environment.Version;
            AppTitle = $"C# WPF ChatGPT v{appVer.Major}.{appVer.Minor} (.NET {dotnetVer.Major}.{dotnetVer.Minor}.{dotnetVer.Build} runtime) by Peter Sun";
#if DEBUG
            AppTitle += " - DEBUG";
#endif
        }

        public string AppTitle { get; }
        public Action<UpdateUIEnum>? UpdateUIAction { get; set; }
        public bool IsCommandNotBusy => !IsCommandBusy;
        [ObservableProperty]
        private bool _isCommandBusy;
        [ObservableProperty]
        private bool _isSendCommandBusy;
        // Wrap _chatHistory.ChatList
        public ObservableCollection<Chat> ChatList { get; }
        [ObservableProperty]
        private Chat _selectedChat;
        [ObservableProperty]
        private string _chatInput;
        [ObservableProperty]
        private string _chatResult = string.Empty;
        [ObservableProperty]
        private Message? _selectedMessage;
        [ObservableProperty]
        private Visibility _imagePaneVisibility = Visibility.Collapsed;
        [ObservableProperty]
        private string _imageInput = "A tennis court";
        [ObservableProperty]
        public byte[]? _resultImage;
        [ObservableProperty]
        private bool _isStreamingMode = true;
        [ObservableProperty]
        string _statusMessage = "Ctrl+Enter for input of multiple lines. Enter-Key to send. Ctrl+UpArrow or Ctrl+DownArrow to navigate previous input lines.";

        // Also RelayCommand from AppBar
        [RelayCommand]
        public void NewChat()
        {
            try
            {
                if (_chatHistory.NewChatExists)
                {
                    // Note: 'New Chat' will be renamed after it's 'used' in Send().
                    StatusMessage = "'New Chat' already exists";
                    return;
                }

                Chat newChat = _chatHistory.AddNewChat();
                ChatList.Add(newChat);
                SelectedChat = newChat;
                UpdateUIAction?.Invoke(UpdateUIEnum.SetFocusToChatInput);

                StatusMessage = "'New Chat' has been added and selected";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
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

        public void CopyChatPrompt(Chat chat)
        {
            try
            {
                Clipboard.SetText(chat.Name);
                StatusMessage = "Chat prompt copied to clipboard";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        public void DeleteChat(Chat chat)
        {
            try
            {
                // Works for deleting last one too
                _chatHistory.DeleteChat(chat.Name);
                ChatList.Remove(chat);
                if (_chatHistory.ChatList.IsEmpty())
                {
                    Chat newChat = _chatHistory.AddNewChat();
                    ChatList.Add(newChat);
                }
                SelectedChat = ChatList[0];

                StatusMessage = "Deleted the chat and selected the first chat in the list";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
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
        public void DeleteMessage(Message message)
        {
            try
            {
                SelectedChat.MessageList.Remove(message);
                StatusMessage = "Message deleted";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
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
            if (SelectedChat.MessageList.IsNotEmpty())
            {
                foreach (Message message in SelectedChat.MessageList)
                {
                    previousPrompts += $"{message.Sender}: {message.Text}";
                }
            }
            return previousPrompts;
        }

        private async Task Send(string prompt, string promptDisplay)
        {
            var newMessage = new Message("Me", promptDisplay, isSenderBot: false);
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
            if (_chatHistory.IsNewChat(SelectedChat.Name))
            {
                // After this call, SelectedChat.Name updated on the left panel because SelectedChat is/wraps the new chat                
                _chatHistory.RenameNewChat(prompt);
            }

            // Handle chat input list / history
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

            if (isSendCommand)
            {
                // Do not change mouse cursor for Send command
                IsSendCommandBusy = isCommandBusy;
            }
            else
            {
                if (IsCommandBusy)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        // partial method (CommunityToolkit MVVM)
        partial void OnSelectedChatChanged(Chat value)
        {
            if (value != null)
            {
                // Re-setup on selected chat changed
                UpdateUIAction?.Invoke(UpdateUIEnum.SetupMessageListViewScrollViewer);
            }
        }
    }
}
