using Core.DataValidators;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISecurityService _securityService;
        public UserService(IUserRepository userRepository, ISecurityService securityService)
        {
            _userRepository = userRepository;
            _securityService = securityService;
        }

        public async Task<bool> Create(CreateUserValidator userInfo)
        {
            var user = new User()
            {
                Email = userInfo.Email
            };

            return await _userRepository.Create(user);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public async Task<User> GetUserById(int id)
        {
            return await _userRepository.GetUserById(id);
        }

        public async Task<bool> UpdateUser(UpdateUserValidator updatedUserInfo)
        {
            return await _securityService.Update(updatedUserInfo);
        }
        public async Task<bool> DeleteUser(int id)
        {
            return await _userRepository.DeleteUser(id);
        }
    }
}
