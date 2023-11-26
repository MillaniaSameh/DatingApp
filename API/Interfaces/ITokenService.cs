using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
  Task<string> CreateUser(AppUser user);
}