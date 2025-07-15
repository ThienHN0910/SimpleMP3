using System.Windows;
using System.Windows.Navigation;
using SimpleMP3.Models;
using SimpleMP3.Views;

namespace SimpleMP3
{
    public partial class MainWindow : Window
    {
        public static User? CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MainFrame.Navigated += MainFrame_Navigated;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
                MainFrame.Navigate(new HomePage(MainFrame));
        }

        private void MainFrame_Navigated(object? sender, NavigationEventArgs e)
        {
            if (MainFrame.Content is LoginPage || MainFrame.Content is RegisterPage)
                GlobalPlayerBar.Visibility = Visibility.Collapsed;
            else
                GlobalPlayerBar.Visibility = Visibility.Visible;
        }
    }
}
