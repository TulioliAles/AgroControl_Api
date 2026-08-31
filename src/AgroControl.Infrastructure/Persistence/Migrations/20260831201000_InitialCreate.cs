using AgroControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroControl.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AgroControlDbContext))]
[Migration("20260831201000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InputCategories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_InputCategories", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Manufacturers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Manufacturers", x => x.Id));

        migrationBuilder.CreateTable(
            name: "MeasurementUnits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                ConversionFactor = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_MeasurementUnits", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AgriculturalInputs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                CommercialName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                Type = table.Column<int>(type: "int", nullable: false),
                CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ManufacturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MeasurementUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AgriculturalInputs", x => x.Id);
                table.ForeignKey("FK_AgriculturalInputs_InputCategories_CategoryId", x => x.CategoryId, "InputCategories", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_AgriculturalInputs_Manufacturers_ManufacturerId", x => x.ManufacturerId, "Manufacturers", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_AgriculturalInputs_MeasurementUnits_MeasurementUnitId", x => x.MeasurementUnitId, "MeasurementUnits", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex("IX_AgriculturalInputs_CategoryId", "AgriculturalInputs", "CategoryId");
        migrationBuilder.CreateIndex("IX_AgriculturalInputs_ManufacturerId", "AgriculturalInputs", "ManufacturerId");
        migrationBuilder.CreateIndex("IX_AgriculturalInputs_MeasurementUnitId", "AgriculturalInputs", "MeasurementUnitId");
        migrationBuilder.CreateIndex("IX_AgriculturalInputs_Name", "AgriculturalInputs", "Name", unique: true);
        migrationBuilder.CreateIndex("IX_InputCategories_Name", "InputCategories", "Name", unique: true);
        migrationBuilder.CreateIndex("IX_Manufacturers_Name", "Manufacturers", "Name", unique: true);
        migrationBuilder.CreateIndex("IX_MeasurementUnits_Symbol", "MeasurementUnits", "Symbol", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AgriculturalInputs");
        migrationBuilder.DropTable(name: "InputCategories");
        migrationBuilder.DropTable(name: "Manufacturers");
        migrationBuilder.DropTable(name: "MeasurementUnits");
    }
}
