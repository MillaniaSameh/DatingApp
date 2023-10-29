using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
  string CreateUser(AppUser user);
}