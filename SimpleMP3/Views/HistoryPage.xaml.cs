using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleMP3.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SimpleMP3.Views
{
    public partial class HistoryPage : Page
    {
        private readonly PlayHistoryService _historyService;

        public HistoryPage()
        {
            InitializeComponent();
            _historyService = App.AppHost.Services.GetRequiredService<PlayHistoryService>();
            Loaded += HistoryPage_Loaded;
        }

        private async void HistoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                GuestPanel.Visibility = Visibility.Visible;
                UserPanel.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                GuestPanel.Visibility = Visibility.Collapsed;
                UserPanel.Visibility = Visibility.Visible;
                UsernameText.Text = App.CurrentUser.Username;
                var historyList = await _historyService.GetHistoryByUserIdAsync(App.CurrentUser.Id);
                HistoryListView.ItemsSource = historyList;
            }
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new HomePage(mainWindow.MainFrame));
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new PlaylistPage());
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new HistoryPage());
        }
        private void Recommend_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new RecommendationPage());
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new LoginPage());
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new LoginPage());
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new RegisterPage());
        }
    }
}
