namespace InventoryTool.Models;

public class InventoryReportLine
{
    public int Id { get; set; }

    public int InventoryReportId { get; set; }
    public InventoryReport InventoryReport { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public InventoryDirection Direction { get; set; }

    public decimal Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum InventoryDirection
{
    In = 1,
    Out = 2
}