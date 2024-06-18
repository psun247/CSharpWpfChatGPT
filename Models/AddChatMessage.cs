namespace CSharpWpfChatGPT.Models
{
    // Used for adding chat from LiveChatViewModel to HistoryViewModel
    public class AddChatMessage
    {
        public AddChatMessage(Chat chat)
        {
            Chat = chat;
        }

        public Chat Chat { get; }
    }
}
