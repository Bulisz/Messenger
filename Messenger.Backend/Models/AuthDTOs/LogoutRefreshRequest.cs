namespace Messenger.Backend.Models.AuthDTOs;

public record LogoutRefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
