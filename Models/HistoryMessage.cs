namespace CSharpWpfChatGPT.Models
{
    // DB counterpart (table) of class Message    
    public class HistoryMessage
    {
        public int Id { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        // The pair auto-defines FK HistoryChatId in HistoryChat table
        public int HistoryChatId { get; set; } // FK, not strictly required
        public HistoryChat HistoryChat { get; set; } = null!;
    }
}
