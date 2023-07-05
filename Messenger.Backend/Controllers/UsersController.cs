using Messenger.Backend.Abstactions;
using Messenger.Backend.MiddlewareConfig;
using Messenger.Backend.Models.AuthDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [HttpGet("getcurrentuser")]
    public async Task<ActionResult<UserDetailsDTO>> GetCurrentUser()
    {
        string currentUserId = User.GetCurrentUserId();
        return Ok(await _userService.GetUserByIdAsync(currentUserId))!;
    }

    [HttpPost(nameof(Register))]
    public async Task<ActionResult<UserDetailsDTO>> Register(CreateUserDTO createUser)
    {
        try
        {
            UserDetailsDTO userDTOget = await _userService.RegisterAsync(createUser);
            return Ok(userDTOget);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost(nameof(Login))]
    public async Task<ActionResult<TokensDTO>> Login(LoginRequest request)
    {
        try
        {
            UserAndRolesDTO userDTOlogin = await _userService.LoginAsync(request);
            TokensDTO loginResponse = await _jwtService.CreateTokensAsync(userDTOlogin.User!);
            return Ok(loginResponse);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
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
