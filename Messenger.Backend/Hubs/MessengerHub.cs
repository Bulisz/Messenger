using Messenger.Backend.Abstactions;
using Messenger.Backend.Models.MessageDTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

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
        string userName = Context.User!.Identity!.Name!;
        string groupName = CreateGroupNameForPrivateMessage(receiverUserName);
        await _messageRepository.JoinToGroupAsync(userName, groupName);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        UserHandler.UserToGroup.TryAdd(userName,groupName);

        if(UserHandler.UnreadedMessages.ContainsKey(userName))
        {
            if(UserHandler.UnreadedMessages[userName].ContainsKey(receiverUserName))
            {
                UserHandler.UnreadedMessages[userName][receiverUserName] = 0;
            }
        }
    }

    public async Task LeavePrivateMessage(string receiverUserName)
    {
        string groupName = CreateGroupNameForPrivateMessage(receiverUserName);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        UserHandler.UserToGroup.TryRemove(Context.User!.Identity!.Name!, out _);
    }
    public async Task SendPrivateMessage(MessageDTO messageDTO)
    {
        string userName = Context.User!.Identity!.Name!;
        string groupName = CreateGroupNameForPrivateMessage(messageDTO.ReceiverName);
        if(UserHandler.UserToGroup.GetValueOrDefault(messageDTO.ReceiverName) != groupName)
        {
            if (UserHandler.UnreadedMessages.TryAdd(messageDTO.ReceiverName, new Dictionary<string, int>()))
            {
                UserHandler.UnreadedMessages[messageDTO.ReceiverName].Add(userName, 1);
            }
            else
            {
                if (!UserHandler.UnreadedMessages[messageDTO.ReceiverName].TryAdd(userName, 1))
                {
                    UserHandler.UnreadedMessages[messageDTO.ReceiverName][userName] += 1;
                }
            }
            await Clients.All.SendAsync("GetUnreadedMessages");
        }

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

    public override async Task<Task> OnConnectedAsync()
    {
        UserHandler.ConnectedIds.TryAdd(Context.ConnectionId, 0);
        await Clients.All.SendAsync("ReceiveConnectedUsersNumber", UserHandler.ConnectedIds.Count);
        return base.OnConnectedAsync();
    }

    public override async Task<Task> OnDisconnectedAsync(Exception? exception)
    {
        UserHandler.ConnectedIds.TryRemove(Context.ConnectionId, out _);
        await Clients.All.SendAsync("ReceiveConnectedUsersNumber", UserHandler.ConnectedIds.Count);
        return base.OnDisconnectedAsync(exception);
    }
}

public static class UserHandler
{
    public static ConcurrentDictionary<string, byte> ConnectedIds = new();
    public static ConcurrentDictionary<string, Dictionary<string, int>> UnreadedMessages = new();
    public static ConcurrentDictionary<string, string> UserToGroup = new();
}
