using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessagRepository messagRepository, IUserRepository userRepository,
 IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];

        if (Context.User == null || string.IsNullOrEmpty(otherUser))
            throw new Exception("Cannot join group");
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdateGroup" , group);

        var messages = await messagRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Could not get user");

        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("You cannot message yourself");

        var sender = await userRepository.GetUSerByUsernameAsync(username);
        var recipient = await userRepository.GetUSerByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null)
            throw new HubException("Cannot send message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUserName = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await messagRepository.GetMessageGroup(groupName);

        if (group != null && group.Connections.Any(x => x.UserName == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if(connections !=null && connections?.Count != null)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                 new {username = sender.UserName, KnownAs = sender.KnownAs});
            }
        }

        messagRepository.AddMessage(message);

        if (await messagRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }
    }
    private async Task<Group> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");
        var group = await messagRepository.GetMessageGroup(groupName);
        var connection = new Connection { ConnectionId = Context.ConnectionId, UserName = username };

        if (group == null)
        {
            group = new Group { Name = groupName };
            messagRepository.AddGroup(group);
        }
        group.Connections.Add(connection);

        if (await messagRepository.SaveAllAsync()) return group;

        throw new HubException("Faild to join group");
    }

    private async Task<Group> RemveFromMessageGroup()
    {
        var group = await messagRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (connection != null && group != null)
        {
            messagRepository.RemoveConnection(connection);
            if(await messagRepository.SaveAllAsync()) return group ;
        }

        throw new Exception("failde to remove from group");
    }

    private string GetGroupName(string caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
