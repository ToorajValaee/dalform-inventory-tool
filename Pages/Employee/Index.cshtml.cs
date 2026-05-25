using InventoryTool.Data;
using InventoryTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryTool.Pages.Employee;

[Authorize(Roles = "Employee")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public int TodayItemCount { get; set; }

    public bool HasDraftReport { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostSubmitTodayAsync()
    {
        var userId = GetUserId();
        var today = DateOnly.FromDateTime(DateTime.Today);

        var report = await _db.InventoryReports
            .Include(x => x.Lines)
            .Where(x =>
                x.EmployeeId == userId &&
                x.ReportDate == today &&
                x.Status == InventoryReportStatus.Draft)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (report == null || !report.Lines.Any())
            return RedirectToPage();

        report.Status = InventoryReportStatus.Submitted;
        report.SubmittedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var userId = GetUserId();
        var today = DateOnly.FromDateTime(DateTime.Today);

        var draftReport = await _db.InventoryReports
            .Include(x => x.Lines)
            .Where(x =>
                x.EmployeeId == userId &&
                x.ReportDate == today &&
                x.Status == InventoryReportStatus.Draft)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (draftReport == null)
        {
            TodayItemCount = 0;
            HasDraftReport = false;
            return;
        }

        TodayItemCount = draftReport.Lines.Count;
        HasDraftReport = draftReport.Lines.Any();
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException();
    }
}