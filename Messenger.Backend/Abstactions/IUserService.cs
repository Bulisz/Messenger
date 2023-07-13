using Messenger.Backend.Models;
using Messenger.Backend.Models.AuthDTOs;

namespace Messenger.Backend.Abstactions;

public interface IUserService
{
    Task<UserAndRolesDTO> LoginAsync(LoginRequest userLoginRequest);
    Task<UserDetailsDTO> RegisterAsync(CreateUserDTO userDTOpost);
    Task<UserDetailsDTO?> GetUserByIdAsync(string id);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<UserAndRolesDTO> RegisterGoogleUserAsync(CreateUserDTO userToCreate);
    Task<IEnumerable<string>> GetUsersAsync();
}