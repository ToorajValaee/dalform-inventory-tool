using Microsoft.AspNetCore.Identity;

namespace InventoryTool.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    // You can add additional properties here if needed
}
