using Leave_Managment_Sytem.Models;

namespace Leave_Managment_Sytem.Services
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(RegisterRequest request);
        Task<string> LoginAsync(LoginRequest request);
    }
}
