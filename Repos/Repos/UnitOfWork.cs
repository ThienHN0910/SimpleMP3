using Repos.Interfaces;
using SimpleMP3.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MusicPlayerDbContext _context;

        public ITrackRepository Tracks { get; }
        public IUserRepository Users { get; }
        public IPlaylistRepository Playlists { get; }

        public UnitOfWork(
            MusicPlayerDbContext context,
            ITrackRepository trackRepo,
            IUserRepository userRepo,
            IPlaylistRepository playlistRepo)
        {
            _context = context;
            Tracks = trackRepo;
            Users = userRepo;
            Playlists = playlistRepo;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
