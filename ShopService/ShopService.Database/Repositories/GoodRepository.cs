using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Database.Repositories
{
	public class GoodRepository : BaseRepository<Good>, IGoodRepository
	{
		public GoodRepository(ShopDbContext context) : base(context)
		{
		}

		protected override Expression<Func<Good, object>> GetOrderByExpression(string orderBy)
		{
			var dict = new Dictionary<string, Expression<Func<Good, object>>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "Title", x => x.Title },
				{ "CreatedAt", x => x.CreatedAt },
				{ "UpdatedAt", x => x.UpdatedAt }
			};

			if (!dict.TryGetValue(orderBy, out var result))
			{
				throw new ArgumentException($"Cant order by {orderBy}");
			}
			return result;
		}

		public async Task<IEnumerable<Good?>> GetGoodsByCategory(Guid categoryId, bool isActual, CancellationToken cancellationToken = default)
		{
			IQueryable<GoodCategoriesAll> query = _context.GoodCategoriesAll
				.Include(x => x.Good)
				.AsNoTracking();

			if (isActual)
			{
				query = query.Where(x => x.ActualCategoryId == categoryId);
			}
			else
			{
				query = query.Where(x => x.FilterCategoryId == categoryId);
			}

			var entities = await query.ToListAsync();
			return entities.Select(x => x.Good);
		}
	}
}
