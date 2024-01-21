using Core.Models;

namespace Core.Interfaces
{
    public interface ISecurityRepository
    {
        Task<bool> Create(Security usersSecuredData);
        Task<Security> GetByUserEmail(string email);
        Task<Security> GetByUserId(int id);
        Task<bool> Update(Security usersSecuredData);
        Task<bool> DeleteByUserId(int id);
    }
}
