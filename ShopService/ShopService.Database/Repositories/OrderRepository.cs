using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Database.Repositories
{
	public class OrderRepository : BaseRepository<Order>, IOrderRepository
	{
		public OrderRepository(ShopDbContext context) : base(context)
		{
		}

		protected override Expression<Func<Order, object>> GetOrderByExpression(string orderBy)
		{
			var dict = new Dictionary<string, Expression<Func<Order, object>>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "CreatedAt", x => x.CreatedAt },
				{ "UpdatedAt", x => x.UpdatedAt }
			};

			if (!dict.TryGetValue(orderBy, out var result))
			{
				throw new ArgumentException($"Cant order by {orderBy}");
			}
			return result;
		}

		public async Task<(int Count, decimal TotalSum)> GetUserOrderStats(Guid userId, CancellationToken cancellationToken = default)
		{
			var result = await _context.Order
				.Where(o => o.Basket.UserId == userId)
				.GroupBy(o => 1)
				.Select(g => new
				{
					Count = g.Count(),
					TotalSum = g.Sum(o => o.FullPrice)
				})
				.FirstOrDefaultAsync(cancellationToken);

			return result != null
				? (result.Count, result.TotalSum)
				: (0, 0);
		}

		public async Task<Dictionary<Guid, UserOrderStats>> GetUsersOrderStats(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
		{
			var statsList = await _context.Order
				.Where(o => userIds.Contains(o.Basket.UserId))
				.GroupBy(o => o.Basket.UserId)
				.Select(g => new UserOrderStats
				{
					UserId = g.Key,
					OrdersCount = g.Count(),
					TotalSum = g.Sum(o => o.FullPrice),
					LastOrderDate = g.Max(o => o.CreatedAt)
				})
				.ToListAsync(cancellationToken);

			return statsList.ToDictionary(s => s.UserId, s => s);
		}

		public async Task<Dictionary<Guid, int>> GetGoodsSoldCount(CancellationToken cancellationToken = default)
		{
			var goodsSold = await _context.Order
				.SelectMany(o => o.Basket.Goods)
				.Where(bi => bi.GoodId.HasValue)
				.GroupBy(bi => bi.GoodId!.Value)
				.Select(g => new
				{
					GoodId = g.Key,
					TotalSold = g.Sum(bi => bi.Count)
				})
				.ToDictionaryAsync(x => x.GoodId, x => x.TotalSold, cancellationToken);

			return goodsSold;
		}
	}
}
