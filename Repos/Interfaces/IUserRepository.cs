using SimpleMP3.Models;
using System.Threading.Tasks;

namespace Repos.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByUsernameAsync(string username);
        Task UpdateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
    }
}
