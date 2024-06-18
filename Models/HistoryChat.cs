using System;
using System.Collections.Generic;

namespace CSharpWpfChatGPT.Models
{
    // DB counterpart (table) of class Chat
    public class HistoryChat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime ModifiedTime { get; set; } = DateTime.Now;
        // Navigation property
        public ICollection<HistoryMessage> MessageList { get; set; } = null!;
    }
}
