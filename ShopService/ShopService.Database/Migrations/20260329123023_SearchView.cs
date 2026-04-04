using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopService.Database.Migrations
{
    /// <inheritdoc />
    public partial class SearchView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateIndex(
                name: "IX_GoodCategory_Description",
                table: "GoodCategory",
                column: "Description")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodCategory_Title",
                table: "GoodCategory",
                column: "Title")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Good_Description",
                table: "Good",
                column: "Description")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Good_Title",
                table: "Good",
                column: "Title")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW ""CategoryGoodSearch"" AS
                SELECT 
                    1 AS ""SourceType"",
                    ""Id"",
                    ""Title"",
                    ""Description"",
                    to_tsvector('simple', COALESCE(""Title"", '') || ' ' || COALESCE(""Description"", '')) AS search_vector,
                    (COALESCE(""Title"", '') || ' ' || COALESCE(""Description"", '')) AS search_text
                FROM ""Good""
                UNION ALL
                SELECT 
                    2 AS ""SourceType"",
                    ""Id"",
                    ""Title"",
                    ""Description"",
                    to_tsvector('simple', COALESCE(""Title"", '') || ' ' || COALESCE(""Description"", '')) AS search_vector,
                    (COALESCE(""Title"", '') || ' ' || COALESCE(""Description"", '')) AS search_text
                FROM ""GoodCategory"";"
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""CategoryGoodSearch""");

			migrationBuilder.DropIndex(
                name: "IX_GoodCategory_Description",
                table: "GoodCategory");

            migrationBuilder.DropIndex(
                name: "IX_GoodCategory_Title",
                table: "GoodCategory");

            migrationBuilder.DropIndex(
                name: "IX_Good_Description",
                table: "Good");

            migrationBuilder.DropIndex(
                name: "IX_Good_Title",
                table: "Good");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
        }
    }
}
