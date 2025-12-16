using Domain.Entities;

namespace Application.IServices.IInternal;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string GenerateRandomToken();
}
