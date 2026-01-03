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

    /// <summary>
    /// Authenticates a user with account lockout protection.
    /// After 5 failed attempts, account is locked for 15 minutes.
    /// </summary>
    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);
        
        if (user == null)
            return null;

        // Check if account is currently locked
        if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            // Account is still locked - return null without updating failed attempts
            return null;
        }

        // If lockout period has passed, reset the lockout
        if (user.LockoutEnd.HasValue && user.LockoutEnd <= DateTime.UtcNow)
        {
            user.LockoutEnd = null;
            user.FailedLoginAttempts = 0;
        }

        var isValid = PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        
        if (isValid)
        {
            // Successful login - reset failed attempts and update last login
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return user;
        }
        else
        {
            // Failed login - increment failed attempts
            user.FailedLoginAttempts++;
            
            // Lock account after 5 failed attempts (configurable via Constants)
            const int maxFailedAttempts = 5;
            const int lockoutMinutes = 15;
            
            if (user.LockoutEnabled && user.FailedLoginAttempts >= maxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutMinutes);
            }
            
            await _context.SaveChangesAsync();
            return null;
        }
    }

    /// <summary>
    /// Checks if an account is currently locked out.
    /// </summary>
    public async Task<(bool IsLocked, DateTime? LockoutEnd)> IsAccountLockedAsync(string username)
    {
        var user = await GetUserByUsernameAsync(username);
        
        if (user == null)
            return (false, null);

        if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            return (true, user.LockoutEnd);
        }

        return (false, null);
    }

    /// <summary>
    /// Unlocks a user account (admin function).
    /// </summary>
    public async Task<bool> UnlockAccountAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
            return false;

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        await _context.SaveChangesAsync();
        
        return true;
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
