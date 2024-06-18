using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CSharpWpfChatGPT.Models
{
    public partial class Chat : ObservableObject
    {
        public Chat(string name)
        {
            Name = name;
            MessageList = new ObservableCollection<Message>();
        }

        // When used, primary key if DB configured, otherwise, unique number used in memory
        public int Id { get; set; }
        // ObservableProperty needed for a new chat name update on the left panel
        [ObservableProperty]
        private string _name = string.Empty;

        // When false, chat is for 'Explain' or 'Translate to'
        public bool IsSend { get; set; } = true;

        public ObservableCollection<Message> MessageList { get; set; }

        public Message AddMessage(string sender, string text)
        {
            Message message = new Message(sender, text);
            AddMessage(message);
            return message;
        }

        public void AddMessage(Message message)
        {
            MessageList.Add(message);
        }
    }
}
