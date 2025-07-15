using Repos.Interfaces;
using SimpleMP3.Models;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<bool> UpdateUsernameAsync(int userId, string newUsername)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            user.Username = newUsername;
            await _unitOfWork.Users.UpdateAsync(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            user.PasswordHash = HashPassword(newPassword);
            await _unitOfWork.Users.UpdateAsync(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            return user != null;
        }

        public async Task<bool> VerifyPasswordAsync(int userId, string inputPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return false;

            string hashedInput = HashPassword(inputPassword);
            return user.PasswordHash == hashedInput;
        }


        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
