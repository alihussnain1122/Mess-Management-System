using MessManagement.Data;
using MessManagement.Helpers;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Services;

public class UserService : IUserService
{
    private readonly MessDbContext _context;

    public UserService(MessDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Member)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Member)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);
        
        if (user == null)
            return null;

        var isValid = PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        
        return isValid ? user : null;
    }

    public async Task<User> CreateUserAsync(string username, string password, UserRole role, string? email = null)
    {
        if (await UserExistsAsync(username))
            throw new ArgumentException("Username already exists", nameof(username));

        var (hash, salt) = PasswordHelper.HashPassword(password);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            return false;

        if (!PasswordHelper.VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalt))
            return false;

        // Check if new password was used before
        if (await IsPasswordInHistoryAsync(userId, newPassword))
            return false;

        // Save current password to history before changing
        var passwordHistory = new PasswordHistory
        {
            UserId = userId,
            PasswordHash = user.PasswordHash,
            PasswordSalt = user.PasswordSalt,
            CreatedAt = DateTime.UtcNow
        };
        _context.PasswordHistories.Add(passwordHistory);

        var (hash, salt) = PasswordHelper.HashPassword(newPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            return false;

        // Check if new password was used before
        if (await IsPasswordInHistoryAsync(userId, newPassword))
            return false;

        // Save current password to history before changing
        var passwordHistory = new PasswordHistory
        {
            UserId = userId,
            PasswordHash = user.PasswordHash,
            PasswordSalt = user.PasswordSalt,
            CreatedAt = DateTime.UtcNow
        };
        _context.PasswordHistories.Add(passwordHistory);

        var (hash, salt) = PasswordHelper.HashPassword(newPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Member)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> IsPasswordInHistoryAsync(int userId, string password)
    {
        // Check current password
        var user = await _context.Users.FindAsync(userId);
        if (user != null && PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            return true;

        // Check password history
        var passwordHistories = await _context.PasswordHistories
            .Where(ph => ph.UserId == userId)
            .ToListAsync();

        foreach (var history in passwordHistories)
        {
            if (PasswordHelper.VerifyPassword(password, history.PasswordHash, history.PasswordSalt))
                return true;
        }

        return false;
    }
}
