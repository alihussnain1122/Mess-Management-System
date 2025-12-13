using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MessManagement.Interfaces;
using MessManagement.Models;
using MessManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MessManagement.Areas.Admin.Pages.Attendance;

[Authorize(Roles = "Admin")]
public class MarkModel : PageModel
{
    private readonly IAttendanceService _attendanceService;
    private readonly IMemberService _memberService;

    public MarkModel(IAttendanceService attendanceService, IMemberService memberService)
    {
        _attendanceService = attendanceService;
        _memberService = memberService;
    }

    public DateTime SelectedDate { get; set; } = DateTime.Today;
    public List<AttendanceMarkViewModel> AttendanceList { get; set; } = new();

    public async Task OnGetAsync(DateTime? date)
    {
        SelectedDate = date ?? DateTime.Today;
        await LoadAttendanceListAsync();
    }

    public async Task<IActionResult> OnPostAsync(DateTime date)
    {
        SelectedDate = date;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var members = await _memberService.GetActiveMembersAsync();

        foreach (var member in members)
        {
            var breakfast = Request.Form[$"breakfast_{member.MemberId}"].ToString() == "on";
            var lunch = Request.Form[$"lunch_{member.MemberId}"].ToString() == "on";
            var dinner = Request.Form[$"dinner_{member.MemberId}"].ToString() == "on";
            
            await _attendanceService.MarkAttendanceAsync(member.MemberId, date, breakfast, lunch, dinner, userId);
        }

        TempData["ToastSuccess"] = $"Attendance for {date:dd MMM yyyy} saved successfully!";
        return RedirectToPage(new { date = date.ToString("yyyy-MM-dd") });
    }

    public async Task<IActionResult> OnPostMarkAllAsync(DateTime date, string mealType)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        
        // Get current attendance to preserve other meal states
        var existingAttendance = await _attendanceService.GetAttendanceForDateAsync(date);
        var members = await _memberService.GetActiveMembersAsync();
        
        foreach (var member in members)
        {
            var existing = existingAttendance.FirstOrDefault(a => a.MemberId == member.MemberId);
            
            bool breakfast = existing?.BreakfastPresent ?? true;
            bool lunch = existing?.LunchPresent ?? true;
            bool dinner = existing?.DinnerPresent ?? true;
            
            // Set the specified meal to present for all
            switch (mealType.ToLower())
            {
                case "breakfast":
                    breakfast = true;
                    break;
                case "lunch":
                    lunch = true;
                    break;
                case "dinner":
                    dinner = true;
                    break;
                case "all":
                    breakfast = lunch = dinner = true;
                    break;
            }
            
            await _attendanceService.MarkAttendanceAsync(member.MemberId, date, breakfast, lunch, dinner, userId);
        }

        TempData["ToastSuccess"] = $"{mealType} marked present for all members!";
        return RedirectToPage(new { date = date.ToString("yyyy-MM-dd") });
    }

    private async Task LoadAttendanceListAsync()
    {
        var members = await _memberService.GetActiveMembersAsync();
        var existingAttendance = await _attendanceService.GetAttendanceForDateAsync(SelectedDate);

        AttendanceList = members.Select(m =>
        {
            var attendance = existingAttendance.FirstOrDefault(a => a.MemberId == m.MemberId);
            return new AttendanceMarkViewModel
            {
                MemberId = m.MemberId,
                MemberName = m.FullName,
                RoomNumber = m.RoomNumber,
                BreakfastPresent = attendance?.BreakfastPresent ?? true,
                LunchPresent = attendance?.LunchPresent ?? true,
                DinnerPresent = attendance?.DinnerPresent ?? true
            };
        }).ToList();
    }
}