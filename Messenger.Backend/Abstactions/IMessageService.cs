using Messenger.Backend.Models.MessageDTOs;

namespace Messenger.Backend.Abstactions;

public interface IMessageService
{
    Task<IEnumerable<MessageDTO>> GetPrivateMessagesAsync(string groupName);
}