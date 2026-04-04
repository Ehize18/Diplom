using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministrativeService.Database.Migrations
{
    /// <inheritdoc />
    public partial class VkId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VkGroupId",
                table: "Shop",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VkGroupId",
                table: "Shop");
        }
    }
}
