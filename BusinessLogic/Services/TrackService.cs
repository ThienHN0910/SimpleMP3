using BusinessLogic.Helpers;
using Repos.Interfaces;
using SimpleMP3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogic.Services
{
    public class TrackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Track>> GetAllTracksAsync()
        {
            return await _unitOfWork.Tracks.GetAllAsync();
        }

        public async Task<Track?> AddTrackFromYouTubeAsync(string url, string saveDirectory)
        {
            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);
            string? videoId = query["v"];
            var existing = await _unitOfWork.Tracks.GetByYouTubeIdAsync(videoId);
            if (existing != null) return null;

            string? path = await YouTubeDownloader.DownloadMp3Async(url, saveDirectory);
            if (path == null) return null;

            string title = Path.GetFileNameWithoutExtension(path);
            
            if(videoId == null)
            {
                throw new Exception("url khong hop le");
            }
            var track = new Track
            {
                Title = title,
                FilePath = path,
                YouTubeId = videoId,
                CreatedAt = DateTime.Now,
                Artist = new Artist
                {
                    Name = "Unknown"
                }
            };

            await _unitOfWork.Artists.AddAsync(track.Artist);

            await _unitOfWork.Tracks.AddAsync(track);
            await _unitOfWork.SaveChangesAsync();

            return track;
        }

        public async Task<List<Track>> SearchTracksAsync(string keyword)
        {
            var all = await _unitOfWork.Tracks.GetAllAsync();
            keyword = keyword.ToLower();

            return all.Where(t =>
                t.Title?.ToLower().Contains(keyword) == true ||
                t.Artist?.Name?.ToLower().Contains(keyword) == true
            ).ToList();
        }
    }
}
