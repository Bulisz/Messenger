using Messenger.Backend.Abstactions;
using Messenger.Backend.Models;
using Messenger.Backend.Models.AuthDTOs;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Backend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserAndRolesDTO> InsertUserAsync(ApplicationUser user, string password)
    {
        IdentityResult createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            throw new ArgumentException("Username or email already exists");
        }

        IdentityResult identityResult = await _userManager.AddToRoleAsync(user, Role.User.ToString());
        if (!identityResult.Succeeded)
        {
            throw new ArgumentException("Invalid role");
        }

        UserAndRolesDTO userDetails = new()
        {
            User = user,
            Roles = await _userManager.GetRolesAsync(user)
        };

        return userDetails;
    }

    public async Task<UserAndRolesDTO> LoginAsync(LoginRequest request)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            throw new ArgumentException("Invalid username");
        };

        bool isPasswordValid = await _userManager.CheckPasswordAsync(user!, request.Password);
        if (!isPasswordValid)
        {
            throw new ArgumentException("Invalid password");
        }

        UserAndRolesDTO userDTOlogin = new()
        {
            User = user,
            Roles = await _userManager.GetRolesAsync(user!)
        };

        return userDTOlogin;
    }


    public async Task<UserAndRolesDTO> GetUserByIdAsync(string id)
    {
        UserAndRolesDTO userDetailsDTO = new();
        ApplicationUser? user = await _userManager.FindByIdAsync(id);
        userDetailsDTO.User = user;
        userDetailsDTO.Roles = await _userManager.GetRolesAsync(user!);

        return userDetailsDTO;
    }
}
