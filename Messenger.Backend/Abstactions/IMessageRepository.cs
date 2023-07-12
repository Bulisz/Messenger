using Messenger.Backend.Models.MessageDTOs;

namespace Messenger.Backend.Abstactions;

public interface IMessageRepository
{
    Task JoinToGroupAsync(string userName, string groupName);
    Task SaveMessageAsync(SaveMessageDTO saveMessageDTO);
}