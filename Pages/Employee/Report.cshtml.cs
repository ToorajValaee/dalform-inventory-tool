using InventoryTool.Data;
using InventoryTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace InventoryTool.Pages.Employee;

[Authorize(Roles = "Employee")]
public class ReportModel : PageModel
{
    private readonly AppDbContext _db;

    public ReportModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public LineInput Input { get; set; } = new();

    public DateOnly ReportDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public InventoryReportStatus ReportStatus { get; set; } = InventoryReportStatus.Draft;
    public List<Product> Products { get; set; } = new();
    public List<InventoryReportLine> Lines { get; set; } = new();

    public class LineInput
    {
        [Required]
        public int? ProductId { get; set; }

        [Required]
        public InventoryDirection Direction { get; set; } = InventoryDirection.In;

        [Required]
        [Range(0.001, 999999999)]
        public decimal Quantity { get; set; }
    }

    public async Task OnGetAsync()
    {
        await LoadPageAsync();
    }

    public async Task<IActionResult> OnPostAddLineAsync()
    {
        await LoadPageAsync();

        if (!ModelState.IsValid)
            return Page();

        var employeeId = GetUserId();

        var report = await GetOrCreateTodayReportAsync(employeeId);

        if (report.Status == InventoryReportStatus.Exported)
            return RedirectToPage();

        var productExists = await _db.Products.AnyAsync(x =>
            x.Id == Input.ProductId &&
            x.IsActive);

        if (!productExists)
        {
            ModelState.AddModelError("Input.ProductId", "کالا اشتباه است.");
            await LoadPageAsync();
            return Page();
        }

        _db.InventoryReportLines.Add(new InventoryReportLine
        {
            InventoryReportId = report.Id,
            ProductId = Input.ProductId!.Value,
            Direction = Input.Direction,
            Quantity = Input.Quantity
        });

        if (report.Status == InventoryReportStatus.Submitted)
        {
            report.Status = InventoryReportStatus.Draft;
            report.SubmittedAt = null;
        }

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteLineAsync(int lineId)
    {
        var employeeId = GetUserId();

        var line = await _db.InventoryReportLines
            .Include(x => x.InventoryReport)
            .FirstOrDefaultAsync(x =>
                x.Id == lineId &&
                x.InventoryReport.EmployeeId == employeeId);

        if (line == null)
            return RedirectToPage();

        if (line.InventoryReport.Status == InventoryReportStatus.Exported)
            return RedirectToPage();

        if (line.InventoryReport.Status == InventoryReportStatus.Submitted)
        {
            line.InventoryReport.Status = InventoryReportStatus.Draft;
            line.InventoryReport.SubmittedAt = null;
        }

        _db.InventoryReportLines.Remove(line);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        var employeeId = GetUserId();
        var report = await GetOrCreateTodayReportAsync(employeeId);

        var hasLine = await _db.InventoryReportLines.AnyAsync(x => x.InventoryReportId == report.Id);

        if (!hasLine)
            return RedirectToPage();

        if (report.Status != InventoryReportStatus.Exported)
        {
            report.Status = InventoryReportStatus.Submitted;
            report.SubmittedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    private async Task LoadPageAsync()
    {
        var employeeId = GetUserId();

        Products = await _db.Products
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .ToListAsync();

        var report = await _db.InventoryReports
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.ReportDate == ReportDate &&
                x.Status != InventoryReportStatus.Exported)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (report != null)
        {
            ReportStatus = report.Status;
            Lines = report.Lines
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }
    }

    private async Task<InventoryReport> GetOrCreateTodayReportAsync(string employeeId)
    {
        var report = await _db.InventoryReports
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.ReportDate == ReportDate &&
                x.Status != InventoryReportStatus.Exported)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (report != null)
            return report;

        report = new InventoryReport
        {
            EmployeeId = employeeId,
            ReportDate = ReportDate,
            Status = InventoryReportStatus.Draft
        };

        _db.InventoryReports.Add(report);
        await _db.SaveChangesAsync();

        return report;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User id not found.");
    }
}