using Messenger.Backend.Abstactions;
using Messenger.Backend.MiddlewareConfig;
using Messenger.Backend.Models.AuthDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Messenger.Backend.Controllers;

[Route("messenger/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;

    public UsersController(IJwtService jwtService, IUserService userService)
    {
        _jwtService = jwtService;
        _userService = userService;
    }

    [Authorize]
    [HttpGet("GetCurrentUser")]
    public async Task<ActionResult<UserDetailsDTO>> GetCurrentUser()
    {
        string currentUserId = User.GetCurrentUserId();
        return Ok(await _userService.GetUserByIdAsync(currentUserId))!;
    }

    [HttpPost(nameof(Register))]
    public async Task<ActionResult<UserDetailsDTO>> Register(CreateUserDTO userDTOpost)
    {
        UserDetailsDTO userDTOget = await _userService.RegisterAsync(userDTOpost);
        return Ok(userDTOget);
    }

    [HttpPost(nameof(Login))]
    public async Task<ActionResult<TokensDTO>> Login(LoginRequest request)
    {
        UserAndRolesDTO userDTOlogin = await _userService.LoginAsync(request);
        TokensDTO loginResponse = await _jwtService.CreateTokensAsync(userDTOlogin.User!);
        return Ok(loginResponse);
    }

    [Authorize]
    [HttpPost(nameof(Logout))]
    public ActionResult Logout(LogoutRefreshRequest logoutRequest)
    {
        _jwtService.ClearRefreshToken(logoutRequest.RefreshToken);
        return Ok();
    }

    [HttpPost(nameof(Refresh))]
    public async Task<ActionResult<TokensDTO>> Refresh(LogoutRefreshRequest refreshRequest)
    {
        try
        {
            TokensDTO authenticationResponse = await _jwtService.RenewTokensAsync(refreshRequest.RefreshToken);
            return Ok(authenticationResponse);
        }
        catch (JwtException)
        {
            return Forbid();
        }
    }
}
