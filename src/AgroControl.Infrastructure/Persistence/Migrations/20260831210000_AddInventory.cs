using System;
using AgroControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroControl.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AgroControlDbContext))]
[Migration("20260831210000_AddInventory")]
public partial class AddInventory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "StockLots",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AgriculturalInputId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LotNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ExpirationDate = table.Column<DateOnly>(type: "date", nullable: true),
                CurrentQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StockLots", x => x.Id);
                table.ForeignKey(
                    name: "FK_StockLots_AgriculturalInputs_AgriculturalInputId",
                    column: x => x.AgriculturalInputId,
                    principalTable: "AgriculturalInputs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "StockMovements",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StockLotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StockMovements", x => x.Id);
                table.ForeignKey(
                    name: "FK_StockMovements_StockLots_StockLotId",
                    column: x => x.StockLotId,
                    principalTable: "StockLots",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_StockLots_AgriculturalInputId_LotNumber",
            table: "StockLots",
            columns: new[] { "AgriculturalInputId", "LotNumber" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_StockLots_ExpirationDate",
            table: "StockLots",
            column: "ExpirationDate");

        migrationBuilder.CreateIndex(
            name: "IX_StockMovements_OccurredAt",
            table: "StockMovements",
            column: "OccurredAt");

        migrationBuilder.CreateIndex(
            name: "IX_StockMovements_StockLotId",
            table: "StockMovements",
            column: "StockLotId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "StockMovements");
        migrationBuilder.DropTable(name: "StockLots");
    }
}
