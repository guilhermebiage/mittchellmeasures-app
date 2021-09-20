using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MMSite6.Migrations
{
    public partial class PricingMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "estimateCost",
                table: "Item",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "dateCreated",
                table: "ASPNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estimateCost",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "dateCreated",
                table: "ASPNetUsers");
        }
    }
}
