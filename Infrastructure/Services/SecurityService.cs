using Core.DataValidators;
using Core.Interfaces;
using Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class SecurityService : ISecurityService
    {
        struct GlobalHashData
        {
            public static readonly int keySize = 64;
            public static readonly int iteration = 350000;
            public static readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        }
        public static string HashPassword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(GlobalHashData.keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                GlobalHashData.iteration,
                GlobalHashData.hashAlgorithm,
                GlobalHashData.keySize);

            return Convert.ToHexString(hash);
        }

        public static bool VerifyPassword(string attemptedPassword, Security usersSecuredData)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(attemptedPassword), Convert.FromHexString(usersSecuredData.Salt), GlobalHashData.iteration, GlobalHashData.hashAlgorithm, GlobalHashData.keySize);
            return hashToCompare.SequenceEqual(Convert.FromHexString(usersSecuredData.HashedPassword));
        }

        private readonly ISecurityRepository _securityRepository;
        public SecurityService(ISecurityRepository securityRepository)
        {
            _securityRepository = securityRepository;
        }

        public async Task<Security> GetByUserEmail(string email)
        {
            return await _securityRepository.GetByUserEmail(email);
        }

        public async Task<Security> GetByUserId(int id)
        {
            return await _securityRepository.GetByUserId(id);
        }

        public async Task<bool> Create(Security usersSecuredData)
        {
            return await _securityRepository.Create(usersSecuredData);
        }

        public async Task<bool> Update(UpdateUserValidator updatedData)
        {
            var originalUser = GetByUserEmail(updatedData.Email).Result;

            originalUser.HashedPassword = HashPassword(updatedData.Password, out byte[] salt);
            originalUser.Salt = Convert.ToHexString(salt);

            return await _securityRepository.Update(originalUser);
        }

        public async Task<bool> DeleteByUserId(int id)
        {
            return await _securityRepository.DeleteByUserId(id);
        }
    }
}
