using SimpleMP3.Models;
using Repos.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class PlayHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlayHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PlayHistory>> GetHistoryByUserIdAsync(int userId, int take = 50)
        {
            return await _unitOfWork.PlayHistories
                .Query()
                .Include(h => h.Track)
                .ThenInclude(t => t.Artist)
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.PlayedAt)
                .Take(take)
                .ToListAsync();
        }
    }
}
