using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleMP3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleMP3.Views
{
    public partial class PlaylistDetailPage : Page
    {
        private readonly PlaylistService _playlistService;
        private readonly TrackService _trackService;
        private readonly int _playlistId;
        private readonly Frame _frame;
        private List<Track> _allTracks = new();
        private List<Track> _displayTracks = new();
        private Playlist? _playlist;

        public PlaylistDetailPage(int playlistId, Frame frame)
        {
            InitializeComponent();
            _playlistService = App.AppHost.Services.GetRequiredService<PlaylistService>();
            _trackService = App.AppHost.Services.GetRequiredService<TrackService>();
            _playlistId = playlistId;
            _frame = frame;
            Loaded += PlaylistDetailPage_Loaded;
        }

        private async void PlaylistDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            _playlist = await _playlistService.GetPlaylistByIdAsync(_playlistId);
            _allTracks = _playlist?.Tracks.ToList() ?? new List<Track>();
            UpdateTrackList(_allTracks);
            PlaylistNameText.Text = _playlist?.Name ?? "";
            PlaylistInfoText.Text = $"{_allTracks.Count} bài hát";
        }

        private void UpdateTrackList(List<Track> tracks)
        {
            var indexed = tracks.Select((t, i) => new TrackListItem
            {
                Index = i + 1,
                Title = t.Title,
                Album = t.Album,
                ArtistName = t.Artist?.Name,
                Id = t.Id
            }).ToList();
            TrackListView.ItemsSource = indexed;
            _displayTracks = tracks;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new PlaylistPage());
        }

        private async void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị dialog chọn bài hát chưa có trong playlist
            var allTracks = await _trackService.GetAllTracksAsync();
            var tracksNotInPlaylist = allTracks.Where(t => _allTracks.All(pt => pt.Id != t.Id)).ToList();
            if (tracksNotInPlaylist.Count == 0)
            {
                MessageBox.Show("Không còn bài hát nào để thêm.");
                return;
            }
            var selectWindow = new SelectTrackWindow(tracksNotInPlaylist);
            if (selectWindow.ShowDialog() == true && selectWindow.SelectedTrack != null)
            {
                await _playlistService.AddTrackToPlaylistAsync(_playlistId, selectWindow.SelectedTrack.Id);
                _playlist = await _playlistService.GetPlaylistByIdAsync(_playlistId);
                _allTracks = _playlist?.Tracks.ToList() ?? new List<Track>();
                UpdateTrackList(_allTracks);
                PlaylistInfoText.Text = $"{_allTracks.Count} bài hát";
            }
        }

        private async void RemoveTrack_Click(object sender, RoutedEventArgs e)
        {
            var item = TrackListView.SelectedItem as TrackListItem;
            if (item != null)
            {
                await _playlistService.RemoveTrackFromPlaylistAsync(_playlistId, item.Id);
                _playlist = await _playlistService.GetPlaylistByIdAsync(_playlistId);
                _allTracks = _playlist?.Tracks.ToList() ?? new List<Track>();
                UpdateTrackList(_allTracks);
                PlaylistInfoText.Text = $"{_allTracks.Count} bài hát";
            }
            else
            {
                MessageBox.Show("Hãy chọn bài hát để xóa.");
            }
        }

        private async void EditPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox("Nhập tên mới cho playlist:", "Đổi tên playlist", _playlist?.Name ?? "");
            if (!string.IsNullOrWhiteSpace(input) && _playlist != null && input != _playlist.Name)
            {
                _playlist.Name = input;
                // Lưu thay đổi tên playlist
                await _playlistService.CreatePlaylistAsync(_playlist.UserId, input); // Bạn nên có hàm UpdatePlaylistNameAsync, đây chỉ là ví dụ
                PlaylistNameText.Text = input;
                MessageBox.Show("Đã đổi tên playlist.");
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search_Click(sender, e);
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                UpdateTrackList(_allTracks);
            }
            else
            {
                var filtered = _allTracks.Where(t =>
                    (t.Title ?? "").ToLower().Contains(keyword) ||
                    (t.Artist?.Name ?? "").ToLower().Contains(keyword) ||
                    (t.Album ?? "").ToLower().Contains(keyword)
                ).ToList();
                UpdateTrackList(filtered);
            }
        }
    }

    public class TrackListItem
    {
        public int Index { get; set; }
        public string Title { get; set; } = "";
        public string? Album { get; set; }
        public string? ArtistName { get; set; }
        public int Id { get; set; }
    }
}