using InventoryTool.Data;
using InventoryTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventoryTool.Pages.Employee;

[Authorize(Roles = "Employee")]
public class SummaryModel : PageModel
{
    private readonly AppDbContext _db;

    public SummaryModel(AppDbContext db)
    {
        _db = db;
    }

    public List<InventoryReport> Reports { get; set; } = new();

    public async Task OnGetAsync()
    {
        var employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        Reports = await _db.InventoryReports
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.ReportDate)
            .Take(30)
            .ToListAsync();
    }
}