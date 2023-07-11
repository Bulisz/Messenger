﻿using Messenger.Backend.Abstactions;
using Messenger.Backend.Hubs;
using Messenger.Backend.MiddlewareConfig;
using Messenger.Backend.Models;
using Messenger.Backend.Models.AuthDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Messenger.Backend.Controllers;

[Route("messenger/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppSettings _applicationSettings;
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;
    private readonly IHubContext<MessengerHub> _hubContext;

    public UsersController(IJwtService jwtService, IUserService userService, IOptions<AppSettings> applicationSettings, IHubContext<MessengerHub> hubContext)
    {
        _applicationSettings = applicationSettings.Value;
        _jwtService = jwtService;
        _userService = userService;
        _hubContext = hubContext;
    }

    [Authorize]
    [HttpGet(nameof(GetCurrentUser))]
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

    [HttpPost(nameof(GoogleLogin))]
    public async Task<ActionResult<TokensDTO>> GoogleLogin(GoogleLoginDTO loginDTO)
    {
        ValidationSettings settings = new()
        {
            Audience = new List<string> { _applicationSettings.ClientId }
        };

        Payload payload = await ValidateAsync(loginDTO.Credential, settings);

        ApplicationUser? user = await _userService.GetUserByEmailAsync(payload.Email);

        if (user != null)
        {
            TokensDTO tokens = await _jwtService.CreateTokensAsync(user);

            return Ok(tokens);
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost(nameof(GoogleRegister))]
    public async Task<ActionResult<TokensDTO>> GoogleRegister(CreateGoogleUserDTO createUser)
    {
        ValidationSettings settings = new()
        {
            Audience = new List<string> { _applicationSettings.ClientId }
        };

        Payload payload = await ValidateAsync(createUser.Credential, settings);

        CreateUserDTO userToCreate = new()
        {
            UserName = createUser.UserName,
            Email = payload.Email
        };

        UserAndRolesDTO createdUser = await _userService.RegisterGoogleUserAsync(userToCreate);
        TokensDTO tokens = await _jwtService.CreateTokensAsync(createdUser.User!);

        return Ok(tokens);
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
