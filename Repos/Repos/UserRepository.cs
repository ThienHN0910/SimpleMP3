using Microsoft.EntityFrameworkCore;
using Repos.Interfaces;
using SimpleMP3.DataAccess;
using SimpleMP3.Models;
using System;
using System.Threading.Tasks;

namespace Repos.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly MusicPlayerDbContext _context;

        public UserRepository(MusicPlayerDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

    }
}
