using System.Threading.Tasks;
using Libraray.Api.DTOs.Auth;

namespace Libraray.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<SignupResponse> SignupAsync(SignupRequest request);
    }
}
