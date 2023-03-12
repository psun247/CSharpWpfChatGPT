using System.Collections.Generic;
using System.Linq;

namespace CSharpWpfChatGPT.Models
{
    public class ChatHistory
    {
        private const string _NewChatName = "New Chat";

        public ChatHistory()
        {
            ChatList = new List<Chat>();
        }

        // On the left panel
        public List<Chat> ChatList { get; }

        public bool NewChatExists => ChatList.Exists(x => x.Name == _NewChatName);
        public bool IsNewChat(string name) => name == _NewChatName;

        public Chat AddNewChat()
        {
            return AddChat(_NewChatName);
        }

        public Chat AddChat(string name)
        {
            var chat = new Chat(name);
            ChatList.Add(chat);
            return chat;
        }

        public void DeleteChat(string name)
        {
            ChatList.RemoveAll(x => x.Name == name);
        }

        public void RenameNewChat(string newName)
        {
            var chat = ChatList.FirstOrDefault(x => x.Name.Equals(_NewChatName));
            if (chat != null)
            {
                int maxToDisplayInSelectedChat = 120;
                if (newName.Length <= maxToDisplayInSelectedChat)
                {                    
                    chat.Name = newName;
                }
                else
                {
                    // NOTE: lose original prompt for a short UI display
                    chat.Name = newName.Substring(0, maxToDisplayInSelectedChat);
                }
            }
        }
    }
}
