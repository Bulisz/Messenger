using Messenger.Backend.Models;
using Messenger.Backend.Models.AuthDTOs;

namespace Messenger.Backend.Abstactions
{
    public interface IUserRepository
    {
        Task<UserAndRolesDTO> InsertUserAsync(ApplicationUser user, string password);
        Task<UserAndRolesDTO> LoginAsync(LoginRequest request);
        Task<UserAndRolesDTO> GetUserByIdAsync(string id);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<UserAndRolesDTO> InsertGoogleUserAsync(ApplicationUser userToRegister);
        Task<IEnumerable<string>> GetUsersAsync();
    }
}