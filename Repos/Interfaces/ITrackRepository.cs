using SimpleMP3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos.Interfaces
{
    public interface ITrackRepository
    {
        Task<List<Track>> GetAllAsync();
        Task<Track?> GetByIdAsync(int id);
        Task<Track?> GetByYouTubeIdAsync(string youtubeId);
        Task AddAsync(Track track);
        void Remove(Track track);
    }
}
