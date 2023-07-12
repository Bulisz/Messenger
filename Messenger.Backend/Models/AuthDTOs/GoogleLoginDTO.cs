namespace Messenger.Backend.Models.AuthDTOs;

public record GoogleLoginDTO
{
    public string Credential { get; set; } = string.Empty;
}
