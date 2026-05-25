using ClosedXML.Excel;
using InventoryTool.Data;
using InventoryTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using InventoryTool.Services;

namespace InventoryTool.Pages.Admin.Reports;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public string? FromDateText { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ToDateText { get; set; }

    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? EmployeeId { get; set; }

    public List<ApplicationUser> Employees { get; set; } = new();

    public List<InventoryReport> Reports { get; set; } = new();

    public async Task OnGetAsync()
    {
        FromDate = PersianDateService.TryParsePersianDate(FromDateText, out var from)
            ? from
            : null;

        ToDate = PersianDateService.TryParsePersianDate(ToDateText, out var to)
            ? to
            : null;

        await LoadAsync();
    }

    public async Task<IActionResult> OnPostExportAsync()
    {
        await LoadAsync();

        var exportReports = Reports
            .Where(x => x.Lines.Any())
            .ToList();

        if (!exportReports.Any())
            return RedirectToPage(new { FromDateText, ToDateText, EmployeeId });

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Inventory Report");

        worksheet.Cell(1, 1).Value = "Date";
        worksheet.Cell(1, 2).Value = "Employee";
        worksheet.Cell(1, 3).Value = "Username";
        worksheet.Cell(1, 4).Value = "Product Code";
        worksheet.Cell(1, 5).Value = "Product Name";
        worksheet.Cell(1, 6).Value = "Type";
        worksheet.Cell(1, 7).Value = "Quantity";

        var row = 2;

        foreach (var report in exportReports.OrderBy(x => x.ReportDate).ThenBy(x => x.Employee.FullName))
        {
            foreach (var line in report.Lines.OrderBy(x => x.Product.Code))
            {
                worksheet.Cell(row, 1).Value = PersianDateService.ToPersianDate(report.ReportDate);
                worksheet.Cell(row, 2).Value = report.Employee.FullName;
                worksheet.Cell(row, 3).Value = report.Employee.UserName;
                worksheet.Cell(row, 4).Value = line.Product.Code;
                worksheet.Cell(row, 5).Value = line.Product.Name;
                worksheet.Cell(row, 6).Value = line.Direction.ToString().ToUpperInvariant();
                worksheet.Cell(row, 7).Value = line.Quantity;
                row++;
            }

            if (report.Status != InventoryReportStatus.Exported)
            {
                report.Status = InventoryReportStatus.Exported;
                report.ExportedAt = DateTime.UtcNow;
            }
        }

        worksheet.Row(1).Style.Font.Bold = true;
        worksheet.Columns().AdjustToContents();

        await _db.SaveChangesAsync();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var fileName = $"filtered-inventory-report-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    private async Task LoadAsync(bool onlySubmittedAndDraft = false)
    {
        Employees = (await _userManager.GetUsersInRoleAsync("Employee"))
            .OrderBy(x => x.FullName)
            .ToList();

        var query = _db.InventoryReports
            .Include(x => x.Employee)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .AsQueryable();

        if (FromDate.HasValue)
            query = query.Where(x => x.ReportDate >= FromDate.Value);

        if (ToDate.HasValue)
            query = query.Where(x => x.ReportDate <= ToDate.Value);

        if (!string.IsNullOrWhiteSpace(EmployeeId))
            query = query.Where(x => x.EmployeeId == EmployeeId);

        if (onlySubmittedAndDraft)
            query = query.Where(x => x.Status != InventoryReportStatus.Exported);

        Reports = await query
            .OrderByDescending(x => x.ReportDate)
            .ThenBy(x => x.Employee.FullName)
            .Take(200)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostExportAccountingAsync()
    {
        var reports = await _db.InventoryReports
            .Include(x => x.Employee)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .Where(x => x.Status != InventoryReportStatus.Exported)
            .Where(x => x.Lines.Any())
            .OrderBy(x => x.ReportDate)
            .ThenBy(x => x.Employee.FullName)
            .ToListAsync();

        var exportLines = reports
            .SelectMany(x => x.Lines)
            .OrderBy(x => x.Product.Code)
            .ToList();

        if (!exportLines.Any())
            return RedirectToPage();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");

        worksheet.RightToLeft = true;

        worksheet.Cell(1, 1).Value = "بارکد";
        worksheet.Cell(1, 2).Value = "کالا";
        worksheet.Cell(1, 3).Value = "مقدار/تعداد";
        worksheet.Cell(1, 4).Value = "مقدار/تعداد واحد دوم";
        worksheet.Cell(1, 5).Value = "فی ورود";
        worksheet.Cell(1, 6).Value = "فی خروج";
        worksheet.Cell(1, 7).Value = "توضیحات کالا";

        var row = 2;

        foreach (var line in exportLines)
        {
            worksheet.Cell(row, 1).Value = line.Product.Code;
            worksheet.Cell(row, 2).Value = line.Product.Name;
            worksheet.Cell(row, 3).Value = line.Quantity;
            worksheet.Cell(row, 4).Value = line.Quantity;
            worksheet.Cell(row, 5).Value = "";
            worksheet.Cell(row, 6).Value = "";
            worksheet.Cell(row, 7).Value = "";
            row++;
        }

        worksheet.Row(1).Style.Font.Bold = true;
        worksheet.Columns().AdjustToContents();

        foreach (var report in reports)
        {
            report.Status = InventoryReportStatus.Exported;
            report.ExportedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var fileName = $"accounting-new-reports-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}