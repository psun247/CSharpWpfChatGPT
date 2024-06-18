using System.Windows;

namespace CSharpWpfChatGPT
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            DataContext = mainViewModel;            
        }
    }
}
