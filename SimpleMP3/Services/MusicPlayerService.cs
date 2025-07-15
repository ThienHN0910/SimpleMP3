using Repos.Interfaces;
using SimpleMP3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace SimpleMP3.Services
{
    public class MusicPlayerService
    {
        private readonly MediaPlayer _mediaPlayer = new();
        private readonly DispatcherTimer _timer = new();
        private readonly IUnitOfWork _unitOfWork;

        public event Action? OnTrackChanged;
        public event Action? OnProgressChanged;

        public Track? CurrentTrack { get; private set; }
        public bool IsPlaying { get; private set; }
        public double Progress => _mediaPlayer.Position.TotalSeconds;
        public double Duration => _mediaPlayer.NaturalDuration.HasTimeSpan
            ? _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds : 0;

        private List<Track>? _playlistTracks;
        private int _currentIndex;

        public MusicPlayerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += (s, e) => OnProgressChanged?.Invoke();
            _mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        }

        public void Play(Track track)
        {
            if (track == null || string.IsNullOrEmpty(track.FilePath) || !File.Exists(track.FilePath))
                return;

            _playlistTracks = null; // Reset playlist mode
            PlayInternal(track);
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

        public void PlayPlaylist(List<Track> tracks)
        {
            if (tracks == null || tracks.Count == 0) return;

            _playlistTracks = tracks;
            _currentIndex = 0;
            PlayInternal(_playlistTracks[_currentIndex]);
        }

        private async void PlayInternal(Track track)
        {
            Stop();

            CurrentTrack = track;
            _mediaPlayer.Open(new Uri(track.FilePath, UriKind.Absolute));
            _mediaPlayer.Position = TimeSpan.Zero;
            _mediaPlayer.Play();
            IsPlaying = true;
            _timer.Start();

            await SavePlayHistoryAsync(track);
            OnTrackChanged?.Invoke();
        }

        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            if (_playlistTracks == null)
            {
                Stop();
                return;
            }

            _currentIndex++;
            if (_currentIndex < _playlistTracks.Count)
            {
                PlayInternal(_playlistTracks[_currentIndex]);
            }
            else
            {
                Stop();
                _playlistTracks = null;
                _currentIndex = 0;
            }
        }

        private async Task SavePlayHistoryAsync(Track track)
        {
            if (App.CurrentUser == null) return;

            var history = new PlayHistory
            {
                UserId = App.CurrentUser.Id,
                TrackId = track.Id,
                PlayedAt = DateTime.Now
            };

            await _unitOfWork.PlayHistories.AddAsync(history);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
