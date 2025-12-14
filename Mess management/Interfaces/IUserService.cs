using MessManagement.Models;

namespace MessManagement.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User> CreateUserAsync(string username, string password, UserRole role, string? email = null);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(int userId, string newPassword);
    Task<bool> UserExistsAsync(string username);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> IsPasswordInHistoryAsync(int userId, string password);
}
