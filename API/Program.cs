using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// app.UseCors(config => config.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
app.UseCors(config => config.WithOrigins("http://localhost:61019").AllowAnyMethod().AllowAnyHeader().AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
  var context = services.GetRequiredService<DataContext>();
  var userManager = services.GetRequiredService<UserManager<AppUser>>();
  var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

  await context.Database.MigrateAsync();
  await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
  await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
  var logger = services.GetService<ILogger<Program>>();
  logger.LogError(ex, "An error occured during migration.");
}

app.Run();
