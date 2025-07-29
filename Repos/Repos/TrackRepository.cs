using Microsoft.EntityFrameworkCore;
using Repos.Interfaces;
using SimpleMP3.DataAccess;
using SimpleMP3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos.Repos
{
    public class TrackRepository : ITrackRepository
    {
        private readonly MusicPlayerDbContext _context;

        public TrackRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Track>> GetAllAsync()
        {
            return await _context.Tracks.Include(t => t.Artist).Include(n => n.UserId).ToListAsync();
        }

        public async Task<Track?> GetByIdAsync(int id)
        {
            return await _context.Tracks.Include(t => t.Artist)
                                        .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Track?> GetByYouTubeIdAsync(string youtubeId)
        {
            return await _context.Tracks.FirstOrDefaultAsync(t => t.YouTubeId == youtubeId);
        }

        public async Task AddAsync(Track track)
        {
            await _context.Tracks.AddAsync(track);
        }
        public async Task<List<Track>> GetByUserIdAsync(int userId)
        {
            return await _context.Tracks
                                 .Where(t => t.UserId == userId)
                                 .Include(t => t.Artist)
                                 .ToListAsync();
        }

        public void Remove(Track track)
        {
            _context.Tracks.Remove(track);
        }
    }
}
