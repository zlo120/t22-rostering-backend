using Core.DataValidators;
using Core.Models;

namespace Core.Interfaces
{
    public interface ISecurityService
    {
        Task<bool> Create(Security usersSecuredData);
        Task<Security> GetByUserEmail(string email);
        Task<Security> GetByUserId(int id);
        Task<bool> Update(UpdateUserValidator updatedUserData);
        Task<bool> DeleteByUserId(int id);
    }
}
