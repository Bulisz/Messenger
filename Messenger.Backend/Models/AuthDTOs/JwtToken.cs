namespace Messenger.Backend.Models.AuthDTOs;

public record JwtToken
{
    public string? Value { get; set; }
    public DateTime Expiration { get; set; }
}
