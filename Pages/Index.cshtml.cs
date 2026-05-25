using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryTool.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToPage("/Account/Login");

        if (User.IsInRole("Admin"))
            return RedirectToPage("/Admin/Index");

        return RedirectToPage("/Employee/Index");
    }
}