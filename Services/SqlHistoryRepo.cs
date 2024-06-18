using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CSharpWpfChatGPT.Models;

namespace CSharpWpfChatGPT.Services
{
    public class SqlHistoryRepo : IHistoryRepo
    {
        public const string SqlConnectionString = "Data Source=localhost\\SQLDev2019;Initial Catalog=CSharpWpfChatGPTDB;Integrated Security=True;Encrypt=False;MultipleActiveResultSets=True";

        private SqlServerContext _sqlServerContext;

        public SqlHistoryRepo()
        {
            _sqlServerContext = new SqlServerContext();
        }

        public string DBConfigInfo => @"SQL Server DB: localhost\SQLDev2019\CSharpWpfChatGPTDB";

        public List<Chat> LoadChatList()
        {
            var list = _sqlServerContext
                            .HistoryChat
                            .Include(x => x.MessageList)
                            .Select(x => new Chat(x.Name)
                            {           
                                Id = x.Id,
                                MessageList = new ObservableCollection<Message>(x.MessageList.Select(y => new Message(y.Sender, y.Text))),
                            }).ToList();
            return list;
        }

        // chat.Id will be updated to created PK
        public void AddChat(Chat chat)
        {
            var historyChat = new HistoryChat
            {
                Name = chat.Name,
                MessageList = chat.MessageList.Select(x =>
                                                new HistoryMessage { Sender = x.Sender, Text = x.Text }).ToList(),
                ModifiedTime = DateTime.Now
            };            
            // This Add will also add historyChat.MessageList to HistoryMessage table
            _sqlServerContext.HistoryChat.Add(historyChat);
            _sqlServerContext.SaveChanges();
            // Important to pass back Id (PK)
            chat.Id = historyChat.Id;
        }

        public void DeleteChat(Chat chat)
        {
            var historyChat = _sqlServerContext.HistoryChat.FirstOrDefault(x => x.Id == chat.Id);
            if (historyChat != null)
            {
                // This Remove will also remove historyChat.MessageList from HistoryMessage table
                _sqlServerContext.HistoryChat.Remove(historyChat);
                _sqlServerContext.SaveChanges();
            }
        }
    }
}