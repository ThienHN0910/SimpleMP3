using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITrackRepository Tracks { get; }
        IUserRepository Users { get; }
        IPlaylistRepository Playlists { get; }
        IPlayHistoryRepository PlayHistories { get; }
        Task<int> SaveChangesAsync();
    }
}
