using API.Data;
using API.interfaces;
using API.Services;
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
       services.AddScoped<IUserRepository,UserRepository>();
       //add user Auto mapper        //the location of the cod 
       services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
       return services ;
    }
}
