namespace Messenger.Backend.Models.MessageDTOs;

public record SaveMessageDTO
{
    public string UserName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
