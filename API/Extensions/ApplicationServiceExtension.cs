using API.Data;
using API.Helpers;
using API.interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
         {
             opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
         });
        //add cors to fix the error for the web 
        services.AddCors();
        //add Token Service
        services.AddScoped<ITokenService, TokenService>();
        //add users service
        services.AddScoped<IUserRepository, UserRepository>();
        //add like 
        services.AddScoped<ILikesRepository, LikesRepository>();
        //messges
        services.AddScoped<IMessagRepository, MessageRepository>();
        //UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        //add photo
        services.AddScoped<IPhotoService, PhotoService>();
        //add service is user online or not
        services.AddScoped<LogUserActivity>();
        //add user Auto mapper        //the location of the cod 
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        //save the photo in cloud 
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        //Add SignalR
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();

        return services;
    }
}
