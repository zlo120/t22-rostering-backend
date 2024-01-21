using Core.Models;

namespace Core.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> Create(User user);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(int id);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
    }
}
