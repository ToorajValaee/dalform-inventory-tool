namespace InventoryTool.Models;

public class InventoryReport
{
    public int Id { get; set; }

    public string EmployeeId { get; set; } = string.Empty;
    public ApplicationUser Employee { get; set; } = null!;

    public DateOnly ReportDate { get; set; }

    public InventoryReportStatus Status { get; set; } = InventoryReportStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ExportedAt { get; set; }

    public List<InventoryReportLine> Lines { get; set; } = new();
}

public enum InventoryReportStatus
{
    Draft = 1,
    Submitted = 2,
    Exported = 3
}