namespace Messenger.Backend.Models.AuthDTOs;

public record UserDetailsDTO
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
