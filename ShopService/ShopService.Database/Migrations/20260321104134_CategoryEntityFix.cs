using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopService.Database.Migrations
{
    /// <inheritdoc />
    public partial class CategoryEntityFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "GoodCategory",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GoodCategory");
        }
    }
}
