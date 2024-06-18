using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CSharpWpfChatGPT.Models;
using CSharpWpfChatGPT.Services;

namespace CSharpWpfChatGPT.History
{
    public partial class HistoryViewModel : ObservableObject
    {
        private IHistoryRepo _historyRepo;

        public HistoryViewModel(IHistoryRepo historyRepo)
        {
            _historyRepo = historyRepo;
            ChatList = new ObservableCollection<Chat>(_historyRepo.LoadChatList());
            RegisterAddChatMessage();
        }

        public string DBConfigInfo => _historyRepo.DBConfigInfo;
        public ObservableCollection<Chat> ChatList { get; }
        [ObservableProperty]
        private Chat? _selectedChat;
        [ObservableProperty]
        string _statusMessage = "List of history chats";       

        private void RegisterAddChatMessage()
        {
            WeakReferenceMessenger.Default.Register<AddChatMessage>(this, (recipient, message) =>
            {
                // Received from LiveChatViewModel to HistoryViewModel
                // WeakReferenceMessenger.Default.Send(new AddChatMessage(chat));
                
                if (message != null)
                {
                    Chat chat = message.Chat;                    
                    Chat? existingChat = ChatList.FirstOrDefault(x => x.Id == chat.Id);
                    if (existingChat != null)
                    {
                        ChatList.Remove(existingChat);
                    }

                    ChatList.Add(chat);
                }
            });
        }

        [RelayCommand]
        private void DeleteHistoryChat(Chat chat)
        {
            try
            {
                if (!ConfirmDelete(chat))
                {
                    return;
                }

                ChatList.Remove(chat);

                _historyRepo.DeleteChat(chat);

                StatusMessage = $"'{chat.Name}' (PK: {chat.Id}) deleted";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        [RelayCommand]
        public void CopyMessage(Message message)
        {
            if (SelectedChat != null)
            {
                int meIndex = SelectedChat.MessageList.IndexOf(message) - 1;
                if (meIndex >= 0)
                {
                    Clipboard.SetText($"Me: {SelectedChat.MessageList[meIndex].Text}\n\n{message.Text}");
                    StatusMessage = "Both Me and Bot messages copied to clipboard";
                }                
            }
        }
        
        private bool ConfirmDelete(Chat chat)
        {
            return MessageBox.Show($"Are you sure you want to delete '{chat.Name}' (PK: {chat.Id})", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
                    == MessageBoxResult.Yes;
        }
    }
}
