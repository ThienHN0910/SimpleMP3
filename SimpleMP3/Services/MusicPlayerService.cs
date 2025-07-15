using SimpleMP3.Models;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace SimpleMP3.Services
{
    public class MusicPlayerService
    {
        private static readonly Lazy<MusicPlayerService> _instance = new(() => new MusicPlayerService());
        public static MusicPlayerService Instance => _instance.Value;

        private readonly MediaPlayer _mediaPlayer = new();
        private readonly DispatcherTimer _timer = new();
        public event Action? OnTrackChanged;
        public event Action? OnProgressChanged;

        public Track? CurrentTrack { get; private set; }
        public bool IsPlaying { get; private set; }
        public double Progress => _mediaPlayer.Position.TotalSeconds;
        public double Duration => _mediaPlayer.NaturalDuration.HasTimeSpan ? _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds : 0;

        private MusicPlayerService()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += (s, e) => OnProgressChanged?.Invoke();
        }

        public void Play(Track track)
        {
            if (track == null || string.IsNullOrEmpty(track.FilePath) || !File.Exists(track.FilePath))
                return;

            // Nếu đang phát bài khác thì dừng lại
            if (IsPlaying)
                Stop();

            CurrentTrack = track;
            _mediaPlayer.Open(new Uri(track.FilePath, UriKind.Absolute));
            _mediaPlayer.Position = TimeSpan.Zero;
            _mediaPlayer.Play();
            IsPlaying = true;
            _timer.Start();
            OnTrackChanged?.Invoke();
        }

        public void PlayOrResume()
        {
            if (CurrentTrack != null)
            {
                _mediaPlayer.Play();
                IsPlaying = true;
                _timer.Start();
                OnTrackChanged?.Invoke();
            }
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
            IsPlaying = false;
            OnTrackChanged?.Invoke();
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
            IsPlaying = false;
            _timer.Stop();
            OnTrackChanged?.Invoke();
        }

        public void Seek(double seconds)
        {
            if (Duration > 0)
                _mediaPlayer.Position = TimeSpan.FromSeconds(seconds);
        }
    }
}