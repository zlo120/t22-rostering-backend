using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly Context _context;

        private readonly ILogger<SecurityRepository> _logger;
        public SecurityRepository(Context context, ILogger<SecurityRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Create(Security usersSecuredData)
        {
            _context.Securities.Add(usersSecuredData);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occured when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }

        public async Task<Security> GetByUserEmail(string email)
        {
            return await _context.Securities.Where(s => s.User.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Security> GetByUserId(int id)
        {
            return await _context.Securities.Where(s => s.User.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> Update(Security usersSecuredData)
        {
            var security = await _context.Securities.Where(s => s.Id == usersSecuredData.Id).FirstOrDefaultAsync();
            if (security is null)
            {
                return false;
            }

            security.User = usersSecuredData.User;
            security.UserId = usersSecuredData.UserId;
            security.Salt = usersSecuredData.Salt;
            security.HashedPassword = usersSecuredData.HashedPassword;

            _context.Securities.Update(security);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occured when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }

        public async Task<bool> DeleteByUserId(int id)
        {
            var security = await GetByUserId(id);
            if (security is null)
            {
                return false;
            }

            _context.Securities.Remove(security);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical error occured when saving changes: {ex}", DateTime.UtcNow.ToLongTimeString());
                return false;
            }
        }
    }
}
