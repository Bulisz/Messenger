using Messenger.Backend.Abstactions;
using Messenger.Backend.Hubs;
using Messenger.Backend.Models.MessageDTOs;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Backend.Controllers;

[Route("messenger/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost(nameof(GetPrivateMessages))]

    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetPrivateMessages(SenderReceiverDTO senderReceiver)
    {
        string groupName = CreateGroupNameForPrivateMessage(senderReceiver.ReceiverUserName, senderReceiver.SenderUserName);
        IEnumerable<MessageDTO> messages = await _messageService.GetPrivateMessagesAsync(groupName);
        return Ok(messages);
    }

    [HttpPost(nameof(GetUnreadedMessages))]

    public ActionResult<IEnumerable<UnreadedMessageDTO>> GetUnreadedMessages(SenderReceiverDTO senderReceiver)
    {
        List<UnreadedMessageDTO> messagNumbers = new();

        Dictionary<string, int>? mesDic = new();
        if(UserHandler.UnreadedMessages.TryGetValue(senderReceiver.SenderUserName, out mesDic))
        {
            mesDic?.ToList().ForEach(kv => messagNumbers.Add(new UnreadedMessageDTO() { SenderName = kv.Key, MessageNumber = kv.Value }));
        }

        return Ok(messagNumbers);
    }

    private static string CreateGroupNameForPrivateMessage(string receiverUserName, string actualUserName)
    {
        string[] groupArray = new string[] { receiverUserName, actualUserName };
        Array.Sort(groupArray);
        return string.Join("", groupArray);
    }
}
