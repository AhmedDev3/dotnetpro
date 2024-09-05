using System;

namespace API.interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IMessagRepository MessagRepository { get; }
    ILikesRepository LikesRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}
