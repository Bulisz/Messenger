namespace Messenger.Backend.Models.AuthDTOs;

public record AppSettings
{
    public string ClientId { get; set; } = string.Empty;
}
