namespace Messenger.Backend.Models.MessageDTOs;

public record SenderReceiverDTO
{
    public string SenderUserName { get; set; } = string.Empty;
    public string ReceiverUserName { get; set; } = string.Empty;
}
