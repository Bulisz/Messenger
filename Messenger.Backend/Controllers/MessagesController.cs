using Messenger.Backend.Abstactions;
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

    private string CreateGroupNameForPrivateMessage(string receiverUserName, string actualUserName)
    {
        string[] groupArray = new string[] { receiverUserName, actualUserName };
        Array.Sort(groupArray);
        return string.Join("", groupArray);
    }
}
