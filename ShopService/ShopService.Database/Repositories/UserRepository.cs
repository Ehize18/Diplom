using System.Linq.Expressions;
using ShopService.Core.Entities;

namespace ShopService.Database.Repositories
{
	public class UserRepository : BaseRepository<User>
	{
		public UserRepository(ShopDbContext context) : base(context)
		{
		}

		protected override Expression<Func<User, object>> GetOrderByExpression(string orderBy)
		{
			var dict = new Dictionary<string, Expression<Func<User, object>>>(StringComparer.OrdinalIgnoreCase)
			{
				{ "Title", x => x.Username },
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
