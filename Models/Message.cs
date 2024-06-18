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
        
        public Visibility CopyButtonVisibility { get; set; }

        public Message(string sender, string text)
        {
            Sender = sender;
            Text = text;            
            CopyButtonVisibility = Sender == "Bot" ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
