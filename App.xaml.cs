using System;
using System.IO;
using System.Windows;
using RestoreWindowPlace;

namespace CSharpWpfChatGPT
{    
    public partial class App : Application
    {
        // Seems work nicely, even for maximized, second monitor
        // https://github.com/Boredbone/RestoreWindowPlace
        private WindowPlace? _windowPlace;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // See README.md for https://platform.openai.com/account/api-keys
                // TODO: a key could look like "sk-Ih...WPd"
                string openaiApiKey = "<Your Open AI API Key>"; 

                var mainWindow = new MainWindow(openaiApiKey);
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

            //if (!File.Exists(windowPlaceConfigFilePath))
            //{
            //    // For the first time, maximize the window, so it won't go off the screen on laptop
            //    // WindowPlacement will take care of future runs
            //    mainWindow.WindowState = WindowState.Maximized;
            //}
        }
    }
}