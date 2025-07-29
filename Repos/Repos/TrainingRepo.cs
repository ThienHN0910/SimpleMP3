using Models;
using Repos.Interfaces;
using SimpleMP3.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos.Repos
{
    public class TrainingRepo : ITrainingRepo
    {
        private readonly MusicPlayerDbContext _context;

        public TrainingRepo(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public List<PlayHistoryEntry> LoadTrainingDataFromEf()
        {
            var query = _context.PlayHistories
            .GroupBy(ph => new { ph.UserId, ph.TrackId })
            .Select(g => new PlayHistoryEntry
            {
                UserId = (uint)g.Key.UserId,
                TrackId = (uint)g.Key.TrackId,
                Label = g.Count() // Số lần nghe
            });

            return query.ToList();
        }
    }
}
