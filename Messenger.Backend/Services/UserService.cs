using Messenger.Backend.Abstactions;
using Messenger.Backend.Models.AuthDTOs;
using Messenger.Backend.Models;

namespace Messenger.Backend.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDetailsDTO> RegisterAsync(CreateUserDTO userDTOpost)
    {
        ApplicationUser userToRegister = new()
        {
            UserName = userDTOpost.UserName,
            Email = userDTOpost.Email,

        };

        UserAndRolesDTO createdUser = await _userRepository.InsertUserAsync(userToRegister, userDTOpost.Password);

        return new UserDetailsDTO { Id = createdUser.User!.Id, Email = createdUser.User!.Email!, Username = createdUser.User!.UserName! };
    }

    public async Task<UserAndRolesDTO> LoginAsync(LoginRequest userLoginRequest)
    {
        return await _userRepository.LoginAsync(userLoginRequest);
    }

    public async Task<UserDetailsDTO?> GetUserByIdAsync(string id)
    {
        UserAndRolesDTO? userDetailsDTO = await _userRepository.GetUserByIdAsync(id);

        return new UserDetailsDTO() { Email = userDetailsDTO.User!.Email!, Id = userDetailsDTO.User.Id, Username = userDetailsDTO.User!.UserName! };
    }
}