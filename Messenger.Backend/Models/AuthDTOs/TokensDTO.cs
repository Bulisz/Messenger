namespace Messenger.Backend.Models.AuthDTOs;

public record TokensDTO
{
    public JwtToken AccessToken { get; set; } = null!;
    public JwtToken RefreshToken { get; set; } = null!;
}
