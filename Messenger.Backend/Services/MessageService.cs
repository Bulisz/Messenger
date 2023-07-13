using Messenger.Backend.Abstactions;
using Messenger.Backend.Models;
using Messenger.Backend.Models.MessageDTOs;

namespace Messenger.Backend.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<IEnumerable<MessageDTO>> GetPrivateMessagesAsync(string groupName)
    {
        IEnumerable<Message> messages = await _messageRepository.GetPrivateMessagesAsync(groupName);

        return messages.Select(m => new MessageDTO
                            {
                                SenderUserName = m.User.UserName!,
                                ReceiverName = groupName,
                                Text = m.Text,
                                CreatedAt = m.CreatedAt
                            });
    }
}
