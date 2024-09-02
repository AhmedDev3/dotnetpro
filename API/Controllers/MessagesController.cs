using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IMessagRepository messagRepository,
 IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot message yourself");

        var sender = await userRepository.GetUSerByUsernameAsync(username);
        var recipient = await userRepository.GetUSerByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null || sender == null || sender.UserName == null || recipient.UserName  == null)
             return BadRequest("Cannot send message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUserName = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        messagRepository.AddMessage(message);

        if (await messagRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

        return BadRequest("Failed to save message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser(
        [FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await messagRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();

        return Ok(await messagRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMesssage(int id)
    {
        var username = User.GetUsername();

        var message = await messagRepository.GetMessage(id);

        if (message == null) return BadRequest("Cannot delet this message");

        if (message.SenderUserName != username && message.RecipientUsername != username)
            //Forbid is 403 error cod 
            return Forbid();

        if (message.SenderUserName == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if(message is {SenderDeleted: true , RecipientDeleted:  true})
        {
            messagRepository.DeleteMessage(message);
        }

        if(await messagRepository.SaveAllAsync()) return Ok();

        return BadRequest ("Problem deleting the message");
    }
}
