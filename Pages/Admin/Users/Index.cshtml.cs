using InventoryTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryTool.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public List<ApplicationUser> Employees { get; set; } = new();

    public async Task OnGetAsync()
    {
        var users = await _userManager.GetUsersInRoleAsync("Employee");

        Employees = users
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.FullName)
            .ToList();
    }
}