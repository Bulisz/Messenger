using Messenger.Backend.Models;
using Messenger.Backend.Models.AuthDTOs;

namespace Messenger.Backend.Abstactions
{
    public interface IUserRepository
    {
        Task<UserAndRolesDTO> InsertUserAsync(ApplicationUser user, string password);
        Task<UserAndRolesDTO> LoginAsync(LoginRequest request);
        Task<UserAndRolesDTO> GetUserByIdAsync(string id);
    }
}