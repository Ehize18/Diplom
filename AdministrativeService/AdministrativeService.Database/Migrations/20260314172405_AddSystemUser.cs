using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministrativeService.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: ["Id", "Username", "CreatedById", "CreatedAt"],
                values: [new Guid("765795f6-ef64-4d68-a517-cfe5208a3e01"), "SYSTEM", new Guid("765795f6-ef64-4d68-a517-cfe5208a3e01"), DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc)]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("User", "Id", new Guid("765795f6-ef64-4d68-a517-cfe5208a3e01"));
        }
    }
}
