using Libraray.Api.Entities;

namespace Libraray.Api.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
