using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Specialized;
using ModernWpf.Controls;
using CSharpWpfChatGPT.Models;

namespace CSharpWpfChatGPT
{   
    public partial class MainWindow : Window
    {
        private const string _NewChat = "New";
        private const string _CopyChatPrompt = "Copy";
        private const string _DeleteChat = "Delete";
        private const string _CopyMessage = "Copy";
        private const string _DeleteMessage = "Delete";

        private ChatViewModel _chatViewModel;
        private ScrollViewer? _chatListViewScrollViewer;
        private ContextMenu _chatContextMenu;
        private ScrollViewer? _messageListViewScrollViewer;
        private ContextMenu _messageContextMenu;

        public MainWindow(string openaiApiKey)
        {
            InitializeComponent();

            DataContext = _chatViewModel = new ChatViewModel(openaiApiKey, (uiUpdateEnum) => UpdateUI(uiUpdateEnum));
            Loaded += MainWindow_Loaded;
            PreviewKeyDown += MainWindow_PreviewKeyDown;
            ChatListView.PreviewMouseRightButtonUp += ChatListView_PreviewMouseRightButtonUp;
            _chatContextMenu = new ContextMenu();
            _chatContextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(ChatMenuOnClick));
            MessageListView.PreviewMouseRightButtonUp += MessageListView_PreviewMouseRightButtonUp;
            _messageContextMenu = new ContextMenu();
            _messageContextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(MessageMenuOnClick));
        }

        // Update UI from ChatViewModel
        private void UpdateUI(UIUpdateEnum uiUpdateEnum)
        {
            switch (uiUpdateEnum)
            {
                case UIUpdateEnum.SetFocusToChatInput:
                    ChatInputTextBox.Focus();
                    break;
                case UIUpdateEnum.SetupMessageListViewScrollViewer:
                    SetupMessageListViewScrollViewer();
                    break;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetupChatListViewScrollViewer();

            _messageListViewScrollViewer = GetScrollViewer(MessageListView);
            SetupMessageListViewScrollViewer();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                // We know the names of arm and rotor TextBoxes on the AlignmentPage, and if we are in either of them, ignore this KeyDown
                TextBox? inputTextBox = Keyboard.FocusedElement as TextBox;
                if (inputTextBox?.Name == "ChatInputTextBox")
                {
                    _chatViewModel.PrevNextChatInput(isUp: e.Key == Key.Up);
                }
            }
        }

        private void SetupChatListViewScrollViewer()
        {
            // Get the ScrollViewer from the ListView that holds the comm log items.
            // We'll need that in order to reliably implement "automatically scroll to 
            // the bottom when new items are added" functionality.            
            _chatListViewScrollViewer = GetScrollViewer(ChatListView);

            // Based on: https://stackoverflow.com/a/1426312	
            INotifyCollectionChanged? notifyCollectionChanged = ChatListView.ItemsSource as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += (sender, e) =>
                {
                    _chatListViewScrollViewer?.ScrollToBottom();
                };
            }
        }

        // Needs to re-setup because MessageListView.ItemsSource resets with SelectedChat.MessageList
        // Note: technically there could be a leak without doing 'CollectionChanged -='
        private void SetupMessageListViewScrollViewer()
        {
            INotifyCollectionChanged? notifyCollectionChanged = MessageListView.ItemsSource as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += (sender, e) =>
                {
                    _messageListViewScrollViewer?.ScrollToBottom();
                };
            }
        }

        private void ChatListView_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Hit test for image, text, blank space below text (Border)
            // Grid is needed when switching from DataGrid to ListView
            if (e.Device.Target is Grid ||
                e.Device.Target is TextBox ||
                e.Device.Target is TextBlock)
            {
                Chat? chat = (e.Device.Target as FrameworkElement)?.DataContext as Chat;
                if (chat != null)
                {
                    ShowChatContextMenu(chat);
                    e.Handled = true;
                }
            }
        }

        private void MessageListView_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Note: target could be System.Windows.Controls.TextBoxView (in .NET 6)
            //          but it's internal (seen in debugger) and not accessible, so use string
            string? target = e.Device.Target?.ToString();

            // Hit test for image, text, blank space below text (Border)
            if (e.Device.Target is Grid ||
                target == "System.Windows.Controls.TextBoxView" ||
                e.Device.Target is TextBlock)
            {
                Message? message = (e.Device.Target as FrameworkElement)?.DataContext as Message;
                if (message != null)
                {
                    ShowMessageContextMenu(message);
                    e.Handled = true;
                }
            }
        }

        // Unicode Characters.
        // In XAML: Glyph = "&#xE107;"        
        // Icon = new FontIcon { Glyph = "\uE107" }
        public void ShowChatContextMenu(Chat chat)
        {
            _chatContextMenu.Tag = chat;
            _chatContextMenu.Items.Clear();

            // Chat header
            _chatContextMenu.Items.Add(new MenuItem
            {
                Header = "Chat",
                IsHitTestVisible = false,
                FontSize = 20,
                FontWeight = FontWeights.SemiBold
            }); ;
            _chatContextMenu.Items.Add(new Separator());
            // New chat
            _chatContextMenu.Items.Add(new MenuItem
            {
                Header = _NewChat,
                FontSize = 20,
                Icon = new FontIcon { Glyph = "\uE8E5" }
            });
            _chatContextMenu.Items.Add(new Separator());
            // Copy chat (prompt only)
            _chatContextMenu.Items.Add(new MenuItem
            {
                Header = _CopyChatPrompt,
                FontSize = 20,
                Icon = new FontIcon { Glyph = "\uE16F" }
            });
            _chatContextMenu.Items.Add(new Separator());
            // Delete chat
            _chatContextMenu.Items.Add(new MenuItem
            {
                Header = _DeleteChat,
                FontSize = 20,
                // In XAML: Glyph = "&#xE107;"
                Icon = new FontIcon { Glyph = "\uE107" }
            });

            _chatContextMenu.IsOpen = true;
        }

        public void ShowMessageContextMenu(Message message)
        {
            _messageContextMenu.Tag = message;
            _messageContextMenu.Items.Clear();

            // Message header
            _messageContextMenu.Items.Add(new MenuItem
            {
                Header = "Message",
                IsHitTestVisible = false,
                FontSize = 20,
                FontWeight = FontWeights.SemiBold
            }); ;
            _messageContextMenu.Items.Add(new Separator());
            // Copy to clipboard
            _messageContextMenu.Items.Add(new MenuItem
            {
                Header = _CopyMessage,
                FontSize = 20,
                Icon = new FontIcon { Glyph = "\uE16F" }
            });
            _messageContextMenu.Items.Add(new Separator());
            _messageContextMenu.Items.Add(new MenuItem
            {
                Header = _DeleteMessage,
                FontSize = 20,
                Icon = new FontIcon { Glyph = "\uE107" }
            });

            _messageContextMenu.IsOpen = true;
        }
        
        private void ChatMenuOnClick(object sender, RoutedEventArgs args)
        {
            MenuItem? mi = args.Source as MenuItem;
            Chat? chat = _chatContextMenu.Tag as Chat;
            if (mi != null && chat != null)
            {
                switch (mi.Header as string)
                {
                    case _NewChat: _chatViewModel.NewChat(); break;
                    case _CopyChatPrompt: _chatViewModel.CopyChatPrompt(chat); break;
                    case _DeleteChat: _chatViewModel.DeleteChat(chat); break;
                    default: break;
                }
            }
        }

        private void MessageMenuOnClick(object sender, RoutedEventArgs args)
        {
            MenuItem? mi = args.Source as MenuItem;
            Message? message = _messageContextMenu.Tag as Message;
            if (mi != null && message != null)
            {
                switch (mi.Header as string)
                {
                    case _CopyMessage: _chatViewModel.CopyMessage(message); break;
                    case _DeleteMessage: _chatViewModel.DeleteMessage(message); break;
                    default: break;
                }
            }
        }

        // From: https://stackoverflow.com/a/41136328
        // This is part of implementing the "automatically scroll to the bottom" functionality.
        private ScrollViewer? GetScrollViewer(UIElement? element)
        {
            ScrollViewer? scrollViewer = null;
            if (element != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && scrollViewer == null; i++)
                {
                    if (VisualTreeHelper.GetChild(element, i) is ScrollViewer)
                    {
                        scrollViewer = (ScrollViewer)(VisualTreeHelper.GetChild(element, i));
                    }
                    else
                    {
                        scrollViewer = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);
                    }
                }
            }
            return scrollViewer;
        }
    }
}
