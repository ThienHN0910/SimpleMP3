using Repos.Interfaces;
using SimpleMP3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (existingUser != null) return false;

            var existingEmail = await _unitOfWork.Users.GetByEmailAsync(email);
            if (existingEmail != null) return false;

            var hashed = HashPassword(password);
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashed,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null) return null;

            var hashed = HashPassword(password);
            return user.PasswordHash == hashed ? user : null;
        }
    }
}
