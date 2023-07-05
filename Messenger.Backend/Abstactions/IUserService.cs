using Messenger.Backend.Models.AuthDTOs;

namespace Messenger.Backend.Abstactions;

public interface IUserService
{
    Task<UserAndRolesDTO> LoginAsync(LoginRequest userLoginRequest);
    Task<UserDetailsDTO> RegisterAsync(CreateUserDTO userDTOpost);
    Task<UserDetailsDTO?> GetUserByIdAsync(string id);
}