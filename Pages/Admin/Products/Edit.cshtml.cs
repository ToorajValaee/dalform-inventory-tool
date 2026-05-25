using InventoryTool.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InventoryTool.Pages.Admin.Products;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public ProductInput Input { get; set; } = new();

    public class ProductInput
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return RedirectToPage("Index");

        Input = new ProductInput
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            IsActive = product.IsActive
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Code = Input.Code.Trim().ToUpperInvariant();
        Input.Name = Input.Name.Trim();

        if (!ModelState.IsValid)
            return Page();

        var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == Input.Id);

        if (product == null)
            return RedirectToPage("Index");

        var duplicate = await _db.Products.AnyAsync(x =>
            x.Id != Input.Id &&
            x.Code == Input.Code);

        if (duplicate)
        {
            ModelState.AddModelError("Input.Code", "This product code already exists.");
            return Page();
        }

        product.Code = Input.Code;
        product.Name = Input.Name;
        product.IsActive = Input.IsActive;

        await _db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}