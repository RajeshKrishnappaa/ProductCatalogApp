using ProdApp.DTOS;

namespace ProdApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> RegisterUserAsync(RegisterDTO dto);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO?> GetUserByNameAsync(string name);
        Task<bool> DeleteUserAsync(int id);
    }
}
