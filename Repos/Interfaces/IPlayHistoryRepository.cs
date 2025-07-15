using SimpleMP3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos.Interfaces
{
    public interface IPlayHistoryRepository
    {
        Task AddAsync(PlayHistory history);
        Task<List<PlayHistory>> GetRecentByUserIdAsync(int userId, int count = 20);
        IQueryable<PlayHistory> Query();
    }

}
