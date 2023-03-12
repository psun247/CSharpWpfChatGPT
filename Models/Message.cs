using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CSharpWpfChatGPT.Models
{
    public partial class Message : ObservableObject
    {
        public string Sender { get; set; }
        // For appending text in Streaming Mode
        [ObservableProperty]
        private string _text = string.Empty;
        
        public Visibility CopyDeleteButtonVisibility { get; set; }

        public Message(string sender, string text, bool isSenderBot)
        {
            Sender = sender;
            Text = text;
            CopyDeleteButtonVisibility = isSenderBot ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
