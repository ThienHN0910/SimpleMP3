using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleMP3.Models;

namespace Repos.Interfaces
{
    public interface IArtistRepository
    {
        Task<Artist?> GetByIdAsync(int id);
        Task<Artist?> GetByNameAsync(string name);
        Task<Artist> AddAsync(Artist artist);
    }
}