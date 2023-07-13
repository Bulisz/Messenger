namespace Messenger.Backend.Models.MessageDTOs;

public record MessageDTO
{
    public string SenderUserName { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
