using System;
using System.IO;
using System.Windows;
using CSharpWpfChatGPT.Services;
using RestoreWindowPlace;

namespace CSharpWpfChatGPT
{
    public partial class App : Application
    {
        private WindowPlace? _windowPlace;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                string openaiApiKey = ParseOpenAIApiKey(e.Args);
                var chatGPTService = new WhetstoneChatGPTService(openaiApiKey);
                var chatViewModel = new ChatViewModel(chatGPTService);
                var mainWindow = new MainWindow(chatViewModel);
                SetupRestoreWindowPlace(mainWindow);
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "CSharpWpfChatGPT will exit on error");
                Current?.Shutdown();
            }
        }

        private string ParseOpenAIApiKey(string[] args)
        {
            if (args?.Length > 0 && args[0].StartsWith('/'))
            {
                // Open AI API Key from command line parameter as "/sk-Ih...WPd" after removing '/'
                return args[0].Remove(0, 1);
            }

            // See README.md for https://platform.openai.com/account/api-keys
            // TODO: put your hard-code key here instead of using a command line parameter
            return "<Your Open AI API Key is something like \"sk-Ih...WPd\">";
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            try
            {
                _windowPlace?.Save();
            }
            catch (Exception)
            {
            }
        }

        private void SetupRestoreWindowPlace(MainWindow mainWindow)
        {
            string windowPlaceConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSharpWpfChatGPTWindowPlace.config");
            _windowPlace = new WindowPlace(windowPlaceConfigFilePath);
            _windowPlace.Register(mainWindow);

            // This logic works but I don't like the window being maximized
            //if (!File.Exists(windowPlaceConfigFilePath))
            //{
            //    // For the first time, maximize the window, so it won't go off the screen on laptop
            //    // WindowPlacement will take care of future runs
            //    mainWindow.WindowState = WindowState.Maximized;
            //}
        }
    }
}