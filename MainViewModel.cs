using System;
using System.Reflection;
using CSharpWpfChatGPT.Services;
using CSharpWpfChatGPT.LiveChat;
using CSharpWpfChatGPT.History;

namespace CSharpWpfChatGPT
{
    public class MainViewModel
    {
        public MainViewModel(IHistoryRepo historyRepo, WhetstoneChatGPTService chatGPTService)
        {
            HistoryViewModel = new HistoryViewModel(historyRepo);
            LiveChatViewModel = new LiveChatViewModel(historyRepo, chatGPTService);
            
            // <Version>1.3</Version> in .csproj
            Version appVer = Assembly.GetExecutingAssembly().GetName().Version!;
            Version dotnetVer = Environment.Version;
            AppTitle = $"C# WPF ChatGPT v{appVer.Major}.{appVer.Minor} (.NET {dotnetVer.Major}.{dotnetVer.Minor}.{dotnetVer.Build} runtime) by Peter Sun";
#if DEBUG
            AppTitle += " - DEBUG";
#endif            
        }

        public string AppTitle { get; }        
        // Bind to LiveChat tab item
        public LiveChatViewModel LiveChatViewModel { get; }
        // Bind to History tab item
        public HistoryViewModel HistoryViewModel { get; }
    }
}
