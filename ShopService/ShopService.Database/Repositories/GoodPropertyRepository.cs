using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Database.Repositories
{
	public class GoodPropertyRepository : BaseRepository<GoodPropertyCategory>, IGoodPropertyRepository
	{
		public GoodPropertyRepository(ShopDbContext context) : base(context)
		{
		}

		protected override Expression<Func<GoodPropertyCategory, object>> GetOrderByExpression(string orderBy)
		{
			var dict = new Dictionary<string, Expression<Func<GoodPropertyCategory, object>>>(StringComparer.OrdinalIgnoreCase)
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

		public async Task<GoodPropertyCategory?> GetGoodProperty(Guid id, bool withValues = false, CancellationToken cancellationToken = default)
		{
			IQueryable<GoodPropertyCategory> query = _context.GoodPropertyCategory;

			if (withValues)
			{
				query = query.Include(x => x.Properties);
			}

			return await query.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<GoodProperty>> GetGoodPropertyValues(Guid propertyId, CancellationToken cancellationToken = default)
		{
			var entities = await _context.GoodProperty
				.Where(x => x.CategoryId == propertyId)
				.ToListAsync(cancellationToken);

			return entities;
		}

		public GoodProperty Create(GoodProperty propertyValue)
		{
			var entry = _context.GoodProperty.Add(propertyValue);
			return entry.Entity;
		}

		public GoodProperty Update(GoodProperty propertyValue)
		{
			var entry = _context.GoodProperty.Update(propertyValue);
			return entry.Entity;
		}

		public async Task<GoodProperty?> GetPropertyValueById(Guid id, CancellationToken cancellationToken = default)
		{
			return await _context.GoodProperty.FirstOrDefaultAsync(x => x.Id == id);
		}
	}
}
