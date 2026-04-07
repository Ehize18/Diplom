using System.Linq.Expressions;
using ShopService.Core.Entities;

namespace ShopService.Database.Repositories
{
	public class OrderRepository : BaseRepository<Order>
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
	}
}
