namespace Messenger.Backend.Models.MessageDTOs;

public record UnreadedMessageDTO
{
    public string SenderName { get; set; } = string.Empty;
    public int MessageNumber { get; set; }
}