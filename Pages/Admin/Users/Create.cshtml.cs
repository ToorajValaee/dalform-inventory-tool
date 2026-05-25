using InventoryTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace InventoryTool.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(4)]
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.FullName = Input.FullName.Trim();
        Input.Username = Input.Username.Trim().ToLowerInvariant();

        if (!ModelState.IsValid)
            return Page();

        var exists = await _userManager.FindByNameAsync(Input.Username);
        if (exists != null)
        {
            ModelState.AddModelError("Input.Username", "This username already exists.");
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Username,
            FullName = Input.FullName,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }

        await _userManager.AddToRoleAsync(user, "Employee");

        return RedirectToPage("Index");
    }
}