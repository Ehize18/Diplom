using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopService.Database.Migrations
{
    /// <inheritdoc />
    public partial class CategoryGoodView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW ""GoodCategoriesAllView"" AS
                WITH RECURSIVE category_tree AS (
                    -- 1. Базовый случай: каждая категория является подкатегорией самой себя
                    SELECT 
                        ""Id"" AS root_category_id,   -- Категория, по которой будем искать
                        ""Id"" AS current_category_id -- Реальная категория товара
                    FROM ""GoodCategory""
    
                    UNION ALL
    
                    -- 2. Рекурсивный шаг: находим все вложенные подкатегории для каждой root_category_id
                    SELECT 
                        ct.root_category_id, 
                        c.""Id""
                    FROM category_tree ct
                    JOIN ""GoodCategory"" c ON c.""ParentCategoryId"" = ct.""current_category_id""
                )
                SELECT 
                    ct.root_category_id AS ""FilterCategoryId"",
                    g.""Id"" AS ""GoodId"",
                    g.""CategoryId"" AS ""ActualCategoryId""
                FROM category_tree ct
                JOIN ""Good"" g ON g.""CategoryId"" = ct.current_category_id;
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""GoodCategoriesAllView""");
        }
    }
}
