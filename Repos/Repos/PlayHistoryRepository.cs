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
    public class PlayHistoryRepository : IPlayHistoryRepository
    {
        private readonly MusicPlayerDbContext _context;

        public PlayHistoryRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }
        public IQueryable<PlayHistory> Query()
        {
            return _context.PlayHistories.AsQueryable();
        }
        public async Task AddAsync(PlayHistory history)
        {
            _context.PlayHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PlayHistory>> GetRecentByUserIdAsync(int userId, int count = 20)
        {
            return await _context.PlayHistories
                .Include(ph => ph.Track)
                .ThenInclude(t => t.Artist)
                .Where(ph => ph.UserId == userId)
                .OrderByDescending(ph => ph.PlayedAt)
                .Take(count)
                .ToListAsync();
        }
    }

}
