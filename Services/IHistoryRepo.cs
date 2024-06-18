using System.Collections.Generic;
using CSharpWpfChatGPT.Models;

namespace CSharpWpfChatGPT.Services
{
    public interface IHistoryRepo
    {
        public string DBConfigInfo { get; }
        public List<Chat> LoadChatList();
        public void AddChat(Chat chat);
        public void DeleteChat(Chat chat);
    }
}
