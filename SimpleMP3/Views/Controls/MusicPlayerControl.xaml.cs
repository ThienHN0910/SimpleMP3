using Microsoft.Extensions.DependencyInjection;
using SimpleMP3.Models;
using SimpleMP3.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleMP3.Views.Controls
{
    public partial class MusicPlayerControl : UserControl
    {
        private bool _isDraggingSlider = false;
        private MusicPlayerService? _player;

        public MusicPlayerControl()
        {
            InitializeComponent();
            Loaded += MusicPlayerControl_Loaded;
        }
        private void MusicPlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            _player = App.AppHost.Services.GetRequiredService<MusicPlayerService>();

            _player.OnTrackChanged += UpdateUI;
            _player.OnProgressChanged += UpdateProgress;
            UpdateUI();
        }
        private void UpdateUI()
        {
            var track = _player.CurrentTrack;

            // Cập nhật tên bài hát và nghệ sĩ
            TrackTitleText.Text = track?.Title ?? "Chưa có bài hát";
            ArtistText.Text = track?.Artist?.Name ?? "Không rõ nghệ sĩ";

            // Cập nhật nút play/pause
            PlayPauseIcon.Text = _player.IsPlaying ? "⏸" : "▶";

            // Cập nhật thời gian và slider
            ProgressSlider.Maximum = _player.Duration;
            ProgressSlider.Value = _player.Progress;

            CurrentTimeText.Text = FormatTime(_player.Progress);
            DurationText.Text = FormatTime(_player.Duration);
        }

        private void UpdateProgress()
        {
            if (_isDraggingSlider) return;


            ProgressSlider.Maximum = _player.Duration;
            ProgressSlider.Value = _player.Progress;

            CurrentTimeText.Text = FormatTime(_player.Progress);
            DurationText.Text = FormatTime(_player.Duration);
        }

        private string FormatTime(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return $"{ts.Minutes}:{ts.Seconds:D2}";
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_player.IsPlaying)
                _player.Pause();
            else
                _player.PlayOrResume();

            UpdateUI();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _player.Stop();
            UpdateUI();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Nếu đang phát playlist thì chuyển bài trước đó
            MessageBox.Show("Tính năng phát bài trước chưa được triển khai.");
        }

        private void ProgressSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDraggingSlider = true;
        }

        private void ProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _player.Seek(ProgressSlider.Value);
            _isDraggingSlider = false;
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDraggingSlider)
            {
                CurrentTimeText.Text = FormatTime(ProgressSlider.Value);
            }
        }
    }
}
