namespace Messenger.Backend.Models.AuthDTOs;

public class CreateGoogleUserDTO
{
    public string UserName { get; set; } = string.Empty;
    public string Credential { get; set; } = string.Empty;
}
