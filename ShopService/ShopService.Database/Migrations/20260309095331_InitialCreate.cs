using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopService.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    TelegramId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Basket",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Basket_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Basket_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Basket_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ParentCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodCategory_GoodCategory_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "GoodCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodCategory_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodCategory_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodPropertyCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodPropertyCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodPropertyCategory_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodPropertyCategory_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullPrice = table.Column<decimal>(type: "money", nullable: false),
                    BasketId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Basket_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Basket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Good",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Price = table.Column<decimal>(type: "money", nullable: false, defaultValue: 0m),
                    OldPrice = table.Column<decimal>(type: "money", nullable: false, defaultValue: 0m),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Good", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Good_GoodCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "GoodCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Good_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Good_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodProperty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodProperty_GoodPropertyCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "GoodPropertyCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodProperty_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodProperty_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodInBasket",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    BasketId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodInBasket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodInBasket_Basket_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Basket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodInBasket_Good_GoodId",
                        column: x => x.GoodId,
                        principalTable: "Good",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodInBasket_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodInBasket_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodGoodProperty",
                columns: table => new
                {
                    GoodsId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertiesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodGoodProperty", x => new { x.GoodsId, x.PropertiesId });
                    table.ForeignKey(
                        name: "FK_GoodGoodProperty_GoodProperty_PropertiesId",
                        column: x => x.PropertiesId,
                        principalTable: "GoodProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodGoodProperty_Good_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Good",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Basket_CreatedById",
                table: "Basket",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_UpdatedById",
                table: "Basket",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_UserId",
                table: "Basket",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Good_CategoryId",
                table: "Good",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Good_CreatedById",
                table: "Good",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Good_UpdatedById",
                table: "Good",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GoodCategory_CreatedById",
                table: "GoodCategory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GoodCategory_ParentCategoryId",
                table: "GoodCategory",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodCategory_UpdatedById",
                table: "GoodCategory",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GoodGoodProperty_PropertiesId",
                table: "GoodGoodProperty",
                column: "PropertiesId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodInBasket_BasketId",
                table: "GoodInBasket",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodInBasket_CreatedById",
                table: "GoodInBasket",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GoodInBasket_GoodId",
                table: "GoodInBasket",
                column: "GoodId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodInBasket_UpdatedById",
                table: "GoodInBasket",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GoodProperty_CategoryId",
                table: "GoodProperty",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodProperty_CreatedById",
                table: "GoodProperty",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GoodProperty_UpdatedById",
                table: "GoodProperty",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GoodPropertyCategory_CreatedById",
                table: "GoodPropertyCategory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GoodPropertyCategory_UpdatedById",
                table: "GoodPropertyCategory",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Order_BasketId",
                table: "Order",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedById",
                table: "Order",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UpdatedById",
                table: "Order",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedById",
                table: "User",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_UpdatedById",
                table: "User",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodGoodProperty");

            migrationBuilder.DropTable(
                name: "GoodInBasket");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "GoodProperty");

            migrationBuilder.DropTable(
                name: "Good");

            migrationBuilder.DropTable(
                name: "Basket");

            migrationBuilder.DropTable(
                name: "GoodPropertyCategory");

            migrationBuilder.DropTable(
                name: "GoodCategory");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
