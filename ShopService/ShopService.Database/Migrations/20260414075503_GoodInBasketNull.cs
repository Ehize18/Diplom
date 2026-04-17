using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopService.Database.Migrations
{
    /// <inheritdoc />
    public partial class GoodInBasketNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodInBasket_Good_GoodId",
                table: "GoodInBasket");

            migrationBuilder.AlterColumn<Guid>(
                name: "GoodId",
                table: "GoodInBasket",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodInBasket_Good_GoodId",
                table: "GoodInBasket",
                column: "GoodId",
                principalTable: "Good",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodInBasket_Good_GoodId",
                table: "GoodInBasket");

            migrationBuilder.AlterColumn<Guid>(
                name: "GoodId",
                table: "GoodInBasket",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodInBasket_Good_GoodId",
                table: "GoodInBasket",
                column: "GoodId",
                principalTable: "Good",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
