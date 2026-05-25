using InventoryTool.Data;
using InventoryTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InventoryTool.Pages.Admin.Products;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public ProductInput Input { get; set; } = new();

    public class ProductInput
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Code = Input.Code.Trim().ToUpperInvariant();
        Input.Name = Input.Name.Trim();

        if (!ModelState.IsValid)
            return Page();

        var exists = await _db.Products.AnyAsync(x => x.Code == Input.Code);

        if (exists)
        {
            ModelState.AddModelError("Input.Code", "This product code already exists.");
            return Page();
        }

        _db.Products.Add(new Product
        {
            Code = Input.Code,
            Name = Input.Name,
            IsActive = true
        });

        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}