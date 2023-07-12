using Messenger.Backend.Abstactions;
using Messenger.Backend.Models.MessageDTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.Backend.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MessengerHub : Hub
{
    private readonly IMessageRepository _messageRepository;

    public MessengerHub(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task JoinPrivateMessage(string receiverUserName)
    {
        string groupName = CreateGroupNameForPrivateMessage(receiverUserName);
        await _messageRepository.JoinToGroupAsync(Context.User!.Identity!.Name!, groupName);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeavePrivateMessage(string receiverUserName)
    {
        string groupName = CreateGroupNameForPrivateMessage(receiverUserName);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
    public async Task SendPrivateMessage(MessageDTO messageDTO)
    {
        string groupName = CreateGroupNameForPrivateMessage(messageDTO.ReceiverName);

        SaveMessageDTO saveMessageDTO = new()
            { 
                UserName = messageDTO.SenderUserName,
                GroupName = groupName,
                Text = messageDTO.Text
            };
        await _messageRepository.SaveMessageAsync(saveMessageDTO);

        await Clients.Group(groupName).SendAsync("ReceiveMessageFromUser", messageDTO);
    }

    private string CreateGroupNameForPrivateMessage(string receiverUserName)
    {
        string[] groupArray = new string[] { receiverUserName, Context.User!.Identity!.Name! };
        Array.Sort(groupArray);
        return string.Join("", groupArray);
    }
}
