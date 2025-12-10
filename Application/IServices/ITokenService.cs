using Domain.Entities;

namespace Application.IServices;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
