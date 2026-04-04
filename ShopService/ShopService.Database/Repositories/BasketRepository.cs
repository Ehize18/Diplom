using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Database.Repositories
{
	public class BasketRepository : BaseRepository<Basket>, IBasketRepository
	{
		public BasketRepository(ShopDbContext context) : base(context)
		{
		}

		protected override Expression<Func<Basket, object>> GetOrderByExpression(string orderBy)
		{
			var dict = new Dictionary<string, Expression<Func<Basket, object>>>(StringComparer.OrdinalIgnoreCase)
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

		public async Task<Basket?> GetCurrentBasket(bool withItems = false, CancellationToken cancellationToken = default)
		{
			var query = _context.Basket
				.AsNoTracking();

			if (withItems)
			{
				query = query
					.Include(x => x.Goods)
					.ThenInclude(x => x.Good);
			}

			return await query
				.FirstOrDefaultAsync(x => x.IsCurrent && x.UserId == _context.DbUser.Id, cancellationToken);
		}

		public async Task<(bool, string)> AddGoodToBasket(Guid basketId, Guid goodId, int count, CancellationToken cancellationToken = default)
		{
			var good = await _context.Good.FirstOrDefaultAsync(x => x.Id == goodId, cancellationToken);

			if (good == null)
			{
				return (false, "good_not_found");
			}

			var basket = await GetByIdAsync(basketId);

			if (basket == null)
			{
				return (false, "basket_not_found");
			}

			AddGoodToBasket(basket, good, count);

			return (true, string.Empty);
		}

		public async Task<(bool, string)> AddGoodToCurrentBasket(Guid goodId, int count, CancellationToken cancellationToken = default)
		{
			var good = await _context.Good.FirstOrDefaultAsync(x => x.Id == goodId, cancellationToken);

			if (good == null)
			{
				return (false, "good_not_found");
			}

			var basket = await GetCurrentBasket(false, cancellationToken);

			if (basket == null)
			{
				basket = new Basket();
				basket.UserId = _context.DbUser.Id;
				basket.IsCurrent = true;
				Create(basket);
			}

			AddGoodToBasket(basket, good, count);

			return (true, string.Empty);
		}

		public async Task<int> ChangeItemCountByCurrentUser(Guid basketItemId, int count, CancellationToken cancellationToken = default)
		{
			try
			{
				return await _context.GoodInBasket
					.Where(x => 
						x.Id == basketItemId &&
						x.Basket!.UserId == _context.DbUser.Id &&
						x.Basket!.IsCurrent)
					.ExecuteUpdateAsync(builder =>
					{
						builder.SetProperty(x => x.Count, count);
						builder.SetProperty(x => x.UpdatedAt, DateTime.UtcNow);
						builder.SetProperty(x => x.UpdatedById, _context.DbUser.Id);
					}, cancellationToken);
			}
			catch (Exception ex)
			{
				return 0;
			}
		}

		public async Task<int> DeleteItemFromBasketByCurrentUser(Guid basketItemId, CancellationToken cancellationToken = default)
		{
			try
			{
				return await _context.GoodInBasket
					.Where(x =>
						x.Id == basketItemId &&
						x.Basket!.UserId == _context.DbUser.Id &&
						x.Basket!.IsCurrent)
					.ExecuteDeleteAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				return 0;
			}
		}

		private void AddGoodToBasket(Basket basket, Good good, int count)
		{
			var goodInBasket = new GoodInBasket()
			{
				BasketId = basket.Id,
				GoodId = good.Id,
				Count = count
			};

			_context.GoodInBasket.Add(goodInBasket);
		}
	}
}
