using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MMSite6.Migrations
{
    public partial class DocumentsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "summary",
                table: "Order",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "desc",
                table: "Order",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "orderDate",
                table: "Order",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Order",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "sqft",
                table: "Item",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    fileId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    documentPath = table.Column<string>(nullable: true),
                    itemId = table.Column<int>(nullable: true),
                    uploadedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.fileId);
                    table.ForeignKey(
                        name: "FK_Documents_Item_itemId",
                        column: x => x.itemId,
                        principalTable: "Item",
                        principalColumn: "itemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_itemId",
                table: "Documents",
                column: "itemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropColumn(
                name: "orderDate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Order");

            migrationBuilder.AlterColumn<string>(
                name: "summary",
                table: "Order",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "desc",
                table: "Order",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "sqft",
                table: "Item",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
