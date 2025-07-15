using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleMP3.Models;
using SimpleMP3.Services;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;

namespace SimpleMP3.Views
{
    public partial class HomePage : Page
    {
        private readonly Frame _frame;
        private readonly TrackService _trackService;
        private List<Track> _tracks = new();

        public HomePage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            _trackService = App.AppHost.Services.GetRequiredService<TrackService>();

            if (App.CurrentUser == null)
            {
                _frame.Navigate(new LoginPage());
                return;
            }
            Loaded += HomePage_Loaded;
            TrackListView.MouseDoubleClick += TrackListView_MouseDoubleClick;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            await ReloadTracksAsync();
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
                MessageBox.Show("Vui lòng nhập YouTube Link");
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
            if (string.IsNullOrEmpty(keyword))
            {
                _tracks = await _trackService.GetAllTracksAsync();
            }
            else
            {
                _tracks = await _trackService.SearchTracksAsync(keyword);
            }
            TrackListView.ItemsSource = _tracks;
            TrackListView.Items.Refresh();
        }

        private async void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search_Click(sender, e);
            }
        }

        private void TrackListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TrackListView.SelectedItem is Track track)
            {
                MusicPlayerService.Instance.Play(track);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            _frame.Navigate(new LoginPage());
        }

        private void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new PlaylistPage());
        }
    }
}
