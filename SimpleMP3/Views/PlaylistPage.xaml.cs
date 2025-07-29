using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleMP3.Models;
using SimpleMP3.Services;
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
        private readonly MusicPlayerService _player;
        private List<Playlist> _playlists = new();

        public PlaylistPage()
        {
            InitializeComponent();
            _playlistService = App.AppHost.Services.GetRequiredService<PlaylistService>();
            _player = App.AppHost.Services.GetRequiredService<MusicPlayerService>();
            Loaded += PlaylistPage_Loaded;
        }

        private async void PlaylistPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                GuestNoticePanel.Visibility = Visibility.Visible;
                PlaylistStackPanel.Visibility = Visibility.Collapsed;
                GuestPanel.Visibility = Visibility.Visible;
                UserPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                GuestNoticePanel.Visibility = Visibility.Collapsed;
                PlaylistStackPanel.Visibility = Visibility.Visible;
                GuestPanel.Visibility = Visibility.Collapsed;
                UserPanel.Visibility = Visibility.Visible;
                UsernameText.Text = App.CurrentUser.Username;
                await ReloadPlaylistsAsync();
            }
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
                var contextMenu = new ContextMenu();

                var playItem = new MenuItem { Header = "Phát playlist" };
                playItem.Click += (s, e) =>
                {
                    _player.PlayPlaylist(playlist.Tracks.ToList());
                };

                var renameItem = new MenuItem { Header = "Đổi tên" };
                renameItem.Click += async (s, e) =>
                {
                    try
                    {
                        var result = await _playlistService.RenamePlaylistAsync(playlist.Id, playlist.Name);
                        if (result)
                        {
                            MessageBox.Show("Đổi tên thành công!");
                            RenderPlaylistCards();
                        }
                        else
                        {
                            MessageBox.Show("Đổi tên thất bại.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}");
                    }
                };

                var deleteItem = new MenuItem { Header = "Xóa" };
                deleteItem.Click += async (s, e) =>
                {
                    try
                    {
                        var result = await _playlistService.DeletePlaylistAsync(playlist.Id);
                        if (result)
                        {
                            _playlists.Remove(playlist);
                            RenderPlaylistCards(); // Gọi trực tiếp vì đang trong UI thread
                        }
                        else
                        {
                            MessageBox.Show("Xóa playlist thất bại.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}");
                    }
                };

                contextMenu.Items.Add(playItem);
                contextMenu.Items.Add(renameItem);
                contextMenu.Items.Add(deleteItem);

                border.ContextMenu = contextMenu;
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