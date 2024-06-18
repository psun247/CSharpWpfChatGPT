using System.Collections.Generic;
using CSharpWpfChatGPT.Models;

namespace CSharpWpfChatGPT.Services
{
    public class EmptyHistoryRepo : IHistoryRepo
    {
        private List<Chat> _chatList;

        public EmptyHistoryRepo()
        {
            _chatList = new List<Chat>();
        }

        public string DBConfigInfo => "Database not configured (list below not saved)";

        public List<Chat> LoadChatList()
        {
            // Uncomment this to insert testing data
            //DevDebugInitializeChatList();

            return _chatList;
        }

        // chat.Id remains as 0
        public void AddChat(Chat chat)
        {            
        }

        public void DeleteChat(Chat chat)
        {            
        }

        private void DevDebugInitializeChatList()
        {
            string prompt = "TestPrompt1";
            string promptDisplay = prompt;
            var newMessage = new Message("Me", promptDisplay);
            var chat = new Chat(prompt);
            chat.AddMessage(newMessage);
            //string result = "TestPrompt1 result";
            chat.AddMessage("Bot", "TestPrompt1 result"); //.Replace("Bot: ", string.Empty));            
            _chatList.Add(chat);

            prompt = "TestPrompt2";
            promptDisplay = prompt;
            newMessage = new Message("Me", promptDisplay);
            chat = new Chat(prompt);
            chat.AddMessage(newMessage);            
            chat.AddMessage("Bot", "TestPrompt2 result");
            _chatList.Add(chat);
        }
    }
}
