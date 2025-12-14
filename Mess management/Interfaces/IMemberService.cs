using MessManagement.Models;

namespace MessManagement.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<Member>> GetAllMembersAsync();
    Task<IEnumerable<Member>> GetActiveMembersAsync();
    Task<Member?> GetMemberByIdAsync(int memberId);
    Task<Member?> GetMemberByUserIdAsync(int userId);
    Task<Member> AddMemberAsync(Member member);
    Task<Member> UpdateMemberAsync(Member member);
    Task<bool> DisableMemberAsync(int memberId);
    Task<bool> EnableMemberAsync(int memberId);
    Task<bool> MemberExistsAsync(int memberId);
    Task<int> GetTotalMembersCountAsync();
    Task<int> GetActiveMembersCountAsync();
    Task<bool> DeleteMemberAsync(int memberId);
}
