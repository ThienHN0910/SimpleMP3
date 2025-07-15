using Repos.Interfaces;
using SimpleMP3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class PlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlaylistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Playlist>> GetAllPlaylistsByUserAsync(int userId)
        {
            return (await _unitOfWork.Playlists.GetAllByUserIdAsync(userId)).ToList();
        }

        public async Task<Playlist?> GetPlaylistByIdAsync(int id)
        {
            return await _unitOfWork.Playlists.GetByIdAsync(id);
        }

        public async Task<bool> CreatePlaylistAsync(int userId, string name)
        {
            var playlist = new Playlist
            {
                UserId = userId,
                Name = name,
                CreatedAt = DateTime.Now,
                IsSystem = false
            };
            await _unitOfWork.Playlists.AddAsync(playlist);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePlaylistAsync(int playlistId)
        {
            var playlist = await _unitOfWork.Playlists.GetByIdAsync(playlistId);
            if (playlist == null) return false;
            _unitOfWork.Playlists.Remove(playlist);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddTrackToPlaylistAsync(int playlistId, int trackId)
        {
            var playlist = await _unitOfWork.Playlists.GetByIdAsync(playlistId);
            var track = await _unitOfWork.Tracks.GetByIdAsync(trackId);
            if (playlist == null || track == null) return false;
            if (!playlist.Tracks.Any(t => t.Id == trackId))
                playlist.Tracks.Add(track);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTrackFromPlaylistAsync(int playlistId, int trackId)
        {
            var playlist = await _unitOfWork.Playlists.GetByIdAsync(playlistId);
            if (playlist == null) return false;
            var track = playlist.Tracks.FirstOrDefault(t => t.Id == trackId);
            if (track != null)
                playlist.Tracks.Remove(track);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}