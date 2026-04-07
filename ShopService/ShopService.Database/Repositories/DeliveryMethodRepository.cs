using System.Linq.Expressions;
using ShopService.Core.Entities;

namespace ShopService.Database.Repositories
{
	public class DeliveryMethodRepository : BaseRepository<DeliveryMethod>
	{
		public DeliveryMethodRepository(ShopDbContext context) : base(context)
		{
		}

		protected override Expression<Func<DeliveryMethod, object>> GetOrderByExpression(string orderBy)
		{
			var dict = new Dictionary<string, Expression<Func<DeliveryMethod, object>>>(StringComparer.OrdinalIgnoreCase)
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
	}
}
