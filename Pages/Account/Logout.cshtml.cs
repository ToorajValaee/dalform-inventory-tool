using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryTool.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SignInManager<InventoryTool.Models.ApplicationUser> _signInManager;

    public LogoutModel(SignInManager<InventoryTool.Models.ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Account/Login");
    }
}