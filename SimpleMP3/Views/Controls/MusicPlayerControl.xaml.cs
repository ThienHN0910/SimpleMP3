using SimpleMP3.Models;
using SimpleMP3.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleMP3.Views.Controls
{
    public partial class MusicPlayerControl : UserControl
    {
        private bool _isDraggingSlider = false;
        public MusicPlayerControl()
        {
            InitializeComponent();
            MusicPlayerService.Instance.OnTrackChanged += UpdateUI;
            MusicPlayerService.Instance.OnProgressChanged += UpdateProgress;
            UpdateUI();
        }

        private void UpdateUI()
        {
            var player = MusicPlayerService.Instance;
            PlayPauseIcon.Text = player.IsPlaying ? "⏸" : "▶";
            ProgressSlider.Maximum = player.Duration;
            ProgressSlider.Value = player.Progress;
        }

        private void UpdateProgress()
        {
            if (!_isDraggingSlider)
            {
                var player = MusicPlayerService.Instance;
                ProgressSlider.Maximum = player.Duration;
                ProgressSlider.Value = player.Progress;
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            var player = MusicPlayerService.Instance;
            if (player.IsPlaying)
                player.Pause();
            else
                player.PlayOrResume();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MusicPlayerService.Instance.Stop();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            // Optional: Implement previous track logic
        }

        private void ProgressSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDraggingSlider = true;
        }

        private void ProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var player = MusicPlayerService.Instance;
            player.Seek(ProgressSlider.Value);
            _isDraggingSlider = false;
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Không làm gì, chỉ update khi thả chuột
        }
    }
}