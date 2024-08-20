using API.Data;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentitySevices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
//error handling
app.UseMiddleware<ExeptionMeddleware>();
//this fix the cors error in the web pol
app.UseCors(x=> x.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("http://localhost:4200","https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// save seed users in the databace
using var scope = app.Services.CreateScope();
var services= scope.ServiceProvider ;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUSers(context);
}
catch (Exception ex)
{
   var logger = services.GetRequiredService<ILogger<Program>>();
   logger.LogError(ex , "An error occurred during migration");
}

app.Run();
