using MessManagement.Data;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace MessManagement.Services;

public class MemberService : IMemberService
{
    private readonly MessDbContext _context;

    public MemberService(MessDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Member>> GetAllMembersAsync()
    {
        return await _context.Members
            .Include(m => m.User)
            .OrderBy(m => m.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
    {
        return await _context.Members
            .Include(m => m.User)
            .Where(m => m.IsActive)
            .OrderBy(m => m.FullName)
            .ToListAsync();
    }

    public async Task<Member?> GetMemberByIdAsync(int memberId)
    {
        return await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.MemberId == memberId);
    }

    public async Task<Member?> GetMemberByUserIdAsync(int userId)
    {
        return await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.UserId == userId);
    }

    public async Task<Member> AddMemberAsync(Member member)
    {
        member.JoinDate = DateTime.UtcNow;
        member.IsActive = true;
        
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        
        return member;
    }

    public async Task<Member> UpdateMemberAsync(Member member)
    {
        var existingMember = await _context.Members.FindAsync(member.MemberId);
        
        if (existingMember == null)
            throw new ArgumentException("Member not found", nameof(member));

        existingMember.FullName = member.FullName;
        existingMember.RoomNumber = member.RoomNumber;
        existingMember.IsActive = member.IsActive;

        await _context.SaveChangesAsync();
        
        return existingMember;
    }

    public async Task<bool> DisableMemberAsync(int memberId)
    {
        var member = await _context.Members.FindAsync(memberId);
        
        if (member == null)
            return false;

        member.IsActive = false;
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> EnableMemberAsync(int memberId)
    {
        var member = await _context.Members.FindAsync(memberId);
        
        if (member == null)
            return false;

        member.IsActive = true;
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> MemberExistsAsync(int memberId)
    {
        return await _context.Members.AnyAsync(m => m.MemberId == memberId);
    }

    public async Task<int> GetTotalMembersCountAsync()
    {
        return await _context.Members.CountAsync();
    }

    public async Task<int> GetActiveMembersCountAsync()
    {
        return await _context.Members.CountAsync(m => m.IsActive);
    }
}
