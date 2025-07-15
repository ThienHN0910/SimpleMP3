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
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly MusicPlayerDbContext _context;

        public PlaylistRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Playlist>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Playlists
                .Include(p => p.Tracks)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Playlist?> GetByIdAsync(int id)
        {
            return await _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
        }

        public void Remove(Playlist playlist)
        {
            _context.Playlists.Remove(playlist);
        }
    }
}
