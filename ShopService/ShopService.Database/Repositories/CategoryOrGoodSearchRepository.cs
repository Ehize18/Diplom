using Microsoft.EntityFrameworkCore;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Database.Repositories
{
	public class CategoryOrGoodSearchRepository : ISearchRepository
	{
		private readonly ShopDbContext _context;

		public CategoryOrGoodSearchRepository(ShopDbContext context)
		{
			_context = context;
		}

		public async Task<List<CategoryOrGoodSearch>> SearchAsync(
		string query,
		int limit = 20,
		double similarityThreshold = 0.2,
		CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return new List<CategoryOrGoodSearch>();
			}

			// Подготовка запроса для to_tsquery (экранирование спецсимволов)
			var ftsQuery = PrepareTsQuery(query);

			var sql = @"
			SELECT 
				""SourceType"",
				""Id"",
				""Title"",
				""Description""
			FROM ""CategoryGoodSearch"",
				 to_tsquery('simple', @p0) AS fts_query
			WHERE ""search_vector"" @@ fts_query
			   OR similarity(""search_text"", @p1) > @p2
			ORDER BY (
				0.7 * ts_rank(""search_vector"", fts_query) +
				0.3 * similarity(""search_text"", @p1)
			) DESC
			LIMIT @p3";

			try
			{
				var results = await _context.CategoryOrGoodSearch
					.FromSqlRaw(sql, ftsQuery, query, similarityThreshold, limit)
					.ToListAsync(cancellationToken);


				return results;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		/// <summary>
		/// Упрощённый поиск только через pg_trgm (быстрее, но менее точно)
		/// </summary>
		public async Task<List<CategoryOrGoodSearch>> SearchSimpleAsync(
			string query,
			int limit = 20,
			CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return new List<CategoryOrGoodSearch>();
			}

			var sql = @"
            SELECT 
                ""SourceType"",
                ""Id"",
                ""Title"",
                ""Description""
            FROM ""CategoryGoodSearch""
            WHERE similarity(""search_text"", @p0) > 0.2
            ORDER BY similarity(""search_text"", @p0) DESC
            LIMIT @p1";

			return await _context.CategoryOrGoodSearch
				.FromSqlRaw(sql, query, limit)
				.ToListAsync(cancellationToken);
		}

		private static string PrepareTsQuery(string query)
		{
			var words = query
				.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(w => w.Replace("'", "''"))
				.Where(w => w.Length > 0)
				.ToArray();

			if (words.Length == 0)
				return string.Empty;

			return string.Join(" & ", words);
		}
	}
}
