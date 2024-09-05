using System;
using API.interfaces;

namespace API.Data;

public class UnitOfWork(DataContext context, IUserRepository userRepository,
 IMessagRepository messagRepository, ILikesRepository likesRepository) : IUnitOfWork
{
    public IUserRepository UserRepository => userRepository;

    public IMessagRepository MessagRepository => messagRepository;

    public ILikesRepository LikesRepository => likesRepository;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
