using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopService.Database.Migrations
{
    /// <inheritdoc />
    public partial class BasketGoodIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GoodInBasket_BasketId",
                table: "GoodInBasket");

            migrationBuilder.CreateIndex(
                name: "Basket_Index",
                table: "GoodInBasket",
                columns: new[] { "BasketId", "GoodId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Basket_Index",
                table: "GoodInBasket");

            migrationBuilder.CreateIndex(
                name: "IX_GoodInBasket_BasketId",
                table: "GoodInBasket",
                column: "BasketId");
        }
    }
}
