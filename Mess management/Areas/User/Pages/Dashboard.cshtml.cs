using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MessManagement.Areas.User.Pages;

[Authorize(Roles = "User")]
public class DashboardModel : PageModel
{
    private readonly IMemberService _memberService;
    private readonly IAttendanceService _attendanceService;
    private readonly IPaymentService _paymentService;
    private readonly IMenuService _menuService;
    private readonly IUserService _userService;

    public DashboardModel(
        IMemberService memberService,
        IAttendanceService attendanceService,
        IPaymentService paymentService,
        IMenuService menuService,
        IUserService userService)
    {
        _memberService = memberService;
        _attendanceService = attendanceService;
        _paymentService = paymentService;
        _menuService = menuService;
        _userService = userService;
    }

    public string MemberName { get; set; } = "";
    public decimal MonthCost { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public int PresentDays { get; set; }
    public IEnumerable<WeeklyMenu> TodayMenu { get; set; } = new List<WeeklyMenu>();
    public IEnumerable<Payment> RecentPayments { get; set; } = new List<Payment>();

    public async Task OnGetAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return;

        var member = await _memberService.GetMemberByUserIdAsync(userId);
        if (member == null) return;

        MemberName = member.FullName;
        var memberId = member.MemberId;

        var now = DateTime.Now;
        
        // Run queries sequentially to avoid DbContext concurrency issues
        PresentDays = await _attendanceService.GetPresentCountForMemberAsync(memberId, now.Month, now.Year);
        var payments = await _paymentService.GetPaymentsForMemberAsync(memberId);
        TodayMenu = await _menuService.GetMenuByDayAsync(DateTime.Today.DayOfWeek);

        TotalPaid = payments.Where(p => p.Date.Month == now.Month && p.Date.Year == now.Year).Sum(p => p.Amount);
        MonthCost = PresentDays * 150m;
        BalanceDue = MonthCost - TotalPaid;
        RecentPayments = payments.OrderByDescending(p => p.Date).Take(5);
    }
}