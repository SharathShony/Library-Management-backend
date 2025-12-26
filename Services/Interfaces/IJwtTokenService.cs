using Libraray.Api.DTO.Users;

namespace Libraray.Api.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserClaimsDto claims);
    }
}
