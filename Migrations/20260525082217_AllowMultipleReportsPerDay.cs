using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTool.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleReportsPerDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryReports_EmployeeId_ReportDate",
                table: "InventoryReports");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReports_EmployeeId_ReportDate",
                table: "InventoryReports",
                columns: new[] { "EmployeeId", "ReportDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryReports_EmployeeId_ReportDate",
                table: "InventoryReports");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReports_EmployeeId_ReportDate",
                table: "InventoryReports",
                columns: new[] { "EmployeeId", "ReportDate" },
                unique: true);
        }
    }
}
