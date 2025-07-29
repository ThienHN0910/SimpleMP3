using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repos.Interfaces;
using SimpleMP3.Models;
using Microsoft.EntityFrameworkCore;
using SimpleMP3.DataAccess;
namespace Repos.Repos
{
    public class ArtistRepo : IArtistRepository 
    {
        private readonly MusicPlayerDbContext _context;

        public ArtistRepo(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public async Task<Artist?> GetByIdAsync(int id)
        {
            return await _context.Artists.FindAsync(id);
        }

        public async Task<Artist?> GetByNameAsync(string name)
        {
            return await _context.Artists.FirstOrDefaultAsync(a => a.Name == name);
        }

        public async Task<Artist> AddAsync(Artist artist)
        {
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            return artist;
        }
    }
}


       