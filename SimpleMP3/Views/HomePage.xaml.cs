using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleMP3.Models;
using SimpleMP3.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleMP3.Views
{
    public partial class HomePage : Page
    {
        private readonly Frame _frame;
        private readonly TrackService _trackService;
        private readonly MusicPlayerService _player;
        private List<Track> _tracks = new();
        private readonly UserService _userService;

        public HomePage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;

            _trackService = App.AppHost.Services.GetRequiredService<TrackService>();
            _player = App.AppHost.Services.GetRequiredService<MusicPlayerService>();
            _userService = App.AppHost.Services.GetRequiredService<UserService>();

            Loaded += HomePage_Loaded;
            TrackListView.MouseDoubleClick += TrackListView_MouseDoubleClick;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            SetupSidebar(); // xử lý sidebar (ẩn/hiện login/logout)
            await ReloadTracksAsync();
        }

        private void SetupSidebar()
        {
            if (App.CurrentUser == null)
            {
                GuestPanel.Visibility = Visibility.Visible;
                UserPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                GuestPanel.Visibility = Visibility.Collapsed;
                UserPanel.Visibility = Visibility.Visible;
                UsernameText.Text = App.CurrentUser.Username;
            }
        }

        private async Task ReloadTracksAsync()
        {
            _tracks = await _trackService.GetAllTracksAsync();
            TrackListView.ItemsSource = _tracks;
            TrackListView.Items.Refresh();
        }

        private async void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            string youtubeId = YouTubeIdBox.Text.Trim();
            if (string.IsNullOrEmpty(youtubeId))
            {
                MessageBox.Show("Vui lòng nhập YouTube ID.");
                return;
            }

            string saveDir = Path.Combine(Directory.GetCurrentDirectory(), "Tracks");
            Directory.CreateDirectory(saveDir);

            var track = await _trackService.AddTrackFromYouTubeAsync(youtubeId, saveDir);
            if (track != null)
            {
                await ReloadTracksAsync();
                MessageBox.Show("Thêm bài hát thành công!");
            }
            else
            {
                MessageBox.Show("Bài hát đã tồn tại hoặc không thể tải.");
            }
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchBox.Text.Trim();
            _tracks = string.IsNullOrEmpty(keyword)
                ? await _trackService.GetAllTracksAsync()
                : await _trackService.SearchTracksAsync(keyword);

            TrackListView.ItemsSource = _tracks;
            TrackListView.Items.Refresh();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search_Click(sender, e);
        }

        private void TrackListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TrackListView.SelectedItem is Track track)
            {
                var tracks = TrackListView.Items.Cast<Track>().ToList();
                int index = tracks.IndexOf(track);
                if (index >= 0)
                {
                    _player.PlayPlaylist(tracks.Skip(index).ToList());
                }
                else
                {
                    MessageBox.Show("Không thể phát bài hát.");
                }
            }
        }
        private void OpenUserPopup_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser != null)
            {
                UserPopup.Visibility = Visibility.Visible;
                NewUsernameBox.Text = App.CurrentUser.Username;
                NewPasswordBox.Password = "";
                ConfirmPasswordBox.Password = "";
            }
        }

        private void CancelUserPopup_Click(object sender, RoutedEventArgs e)
        {
            UserPopup.Visibility = Visibility.Collapsed;
        }

        private async void SaveUserInfo_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = NewUsernameBox.Text.Trim();
            string oldPassword = OldPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(newUsername))
            {
                MessageBox.Show("Tên người dùng không được để trống.");
                return;
            }

            bool hasChange = false;

            // Kiểm tra đổi tên
            if (newUsername != App.CurrentUser.Username)
            {
                bool usernameTaken = await _userService.IsUsernameTakenAsync(newUsername);
                if (usernameTaken)
                {
                    MessageBox.Show("Tên người dùng đã tồn tại.");
                    return;
                }

                var result = await _userService.UpdateUsernameAsync(App.CurrentUser.Id, newUsername);
                if (result)
                {
                    App.CurrentUser.Username = newUsername;
                    UsernameText.Text = newUsername;
                    hasChange = true;
                }
            }

            // Kiểm tra đổi mật khẩu nếu có nhập
            if (!string.IsNullOrWhiteSpace(oldPassword) ||
                !string.IsNullOrWhiteSpace(newPassword) ||
                !string.IsNullOrWhiteSpace(confirmPassword))
            {
                if (string.IsNullOrWhiteSpace(oldPassword))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu hiện tại.");
                    return;
                }

                bool isCorrect = await _userService.VerifyPasswordAsync(App.CurrentUser.Id, oldPassword);
                if (!isCorrect)
                {
                    MessageBox.Show("Mật khẩu hiện tại không chính xác.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu mới và xác nhận.");
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp.");
                    return;
                }

                var result = await _userService.UpdatePasswordAsync(App.CurrentUser.Id, newPassword);
                if (result) hasChange = true;
            }

            if (hasChange)
            {
                MessageBox.Show("Cập nhật thành công!");
                UserPopup.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Không có thay đổi nào.");
            }
        }



        // ==== SIDEBAR ====

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new LoginPage());
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new RegisterPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            _frame.Navigate(new LoginPage());
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new HistoryPage());
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new PlaylistPage());
        }
    }
}
