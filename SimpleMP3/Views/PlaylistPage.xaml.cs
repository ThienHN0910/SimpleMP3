using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleMP3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleMP3.Views
{
    public partial class PlaylistPage : Page
    {
        private readonly PlaylistService _playlistService;
        private List<Playlist> _playlists = new();

        public PlaylistPage()
        {
            InitializeComponent();
            _playlistService = App.AppHost.Services.GetRequiredService<PlaylistService>();
            Loaded += PlaylistPage_Loaded;
        }

        private async void PlaylistPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ReloadPlaylistsAsync();
        }

        private async System.Threading.Tasks.Task ReloadPlaylistsAsync()
        {
            if (App.CurrentUser != null)
            {
                _playlists = await _playlistService.GetAllPlaylistsByUserAsync(App.CurrentUser.Id);
                RenderPlaylistCards();
            }
        }

        private void RenderPlaylistCards()
        {
            PlaylistWrapPanel.Children.Clear();
            foreach (var playlist in _playlists)
            {
                var border = new Border
                {
                    Width = 200,
                    Height = 120,
                    Margin = new Thickness(12),
                    CornerRadius = new CornerRadius(12),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#222326")),
                    Cursor = Cursors.Hand,
                    Tag = playlist
                };

                var stack = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var nameText = new TextBlock
                {
                    Text = playlist.Name,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Foreground = Brushes.White,
                    TextAlignment = TextAlignment.Center,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                stack.Children.Add(nameText);

                if (playlist.Tracks != null && playlist.Tracks.Count > 0)
                {
                    var firstTrackText = new TextBlock
                    {
                        Text = playlist.Tracks.FirstOrDefault().Title,
                        FontSize = 13,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B3B3B3")),
                        Margin = new Thickness(0, 8, 0, 0),
                        TextAlignment = TextAlignment.Center,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    };
                    stack.Children.Add(firstTrackText);
                }

                border.Child = stack;
                border.MouseLeftButtonUp += PlaylistCard_Click;
                PlaylistWrapPanel.Children.Add(border);
            }
        }

        private void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            CreatePlaylistPopup.Visibility = Visibility.Visible;
            PlaylistNameBox.Text = "";
        }

        private void CancelCreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            CreatePlaylistPopup.Visibility = Visibility.Collapsed;
        }

        private async void CreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            string name = PlaylistNameBox.Text.Trim();
            if (!string.IsNullOrEmpty(name) && App.CurrentUser != null)
            {
                await _playlistService.CreatePlaylistAsync(App.CurrentUser.Id, name);
                await ReloadPlaylistsAsync();
                CreatePlaylistPopup.Visibility = Visibility.Collapsed;
            }
        }

        private void PlaylistCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Playlist playlist)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(new PlaylistDetailPage(playlist.Id, mainWindow.MainFrame));
                }
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new HomePage(mainWindow.MainFrame));
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new LoginPage());
            }
        }
    }
}