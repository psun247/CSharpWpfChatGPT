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
                // See README.md for https://platform.openai.com/account/api-keys
                // TODO: a key could look like "sk-Ih...WPd"
                string openaiApiKey = "<Your Open AI API Key>";

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