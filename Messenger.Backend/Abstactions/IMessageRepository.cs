using Messenger.Backend.Models;
using Messenger.Backend.Models.MessageDTOs;

namespace Messenger.Backend.Abstactions;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetPrivateMessagesAsync(string groupName);
    Task JoinToGroupAsync(string userName, string groupName);
    Task SaveMessageAsync(SaveMessageDTO saveMessageDTO);
}