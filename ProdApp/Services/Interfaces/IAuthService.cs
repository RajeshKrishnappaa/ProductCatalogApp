using ProdApp.DTOS;

namespace ProdApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginDTO dto);
    }
}
