using Shared.RabbitMQ.Contracts;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Application.Services
{
	public class GoodService
	{
		private readonly IBaseRepository<Good> _goodRepository;

		public GoodService(IBaseRepository<Good> goodRepository)
		{
			_goodRepository = goodRepository;
		}

		public async Task<Good?> CreateGoodAsync(UpdateGood update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _goodRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return null;
			}

			try
			{
				var category = new Good
				{
					Title = update.GoodTitle!,
					Description = update.GoodDescription != null ? update.GoodDescription : string.Empty,
					CategoryId = update.CategoryId,
					Count = update.Count,
					Price = update.Price,
					OldPrice = update.OldPrice
				};
				await _goodRepository.SetUser(update.UpdatedById);
				var result = _goodRepository.Create(category);
				await _goodRepository.SaveChangesAsync(cancellationToken);
				return result;
			}
			catch (Exception ex)
			{
			}

			return null;
		}

		public async Task<Good?> UpdateGoodAsync(UpdateGood update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _goodRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return null;
			}

			try
			{
				await _goodRepository.SetUser(update.UpdatedById);
				var good = await _goodRepository.GetByIdAsync((Guid)update.CategoryId!);
				if (good == null)
				{
					return null;
				}
				good.Title = update.GoodTitle;
				good.Description = update.GoodDescription != null ? update.GoodDescription : string.Empty;
				good.CategoryId = update.CategoryId;
				good.Count = update.Count;
				good.Price = update.Price;
				good.OldPrice = update.OldPrice;
				_goodRepository.Update(good);
				await _goodRepository.SaveChangesAsync(cancellationToken);
				return good;
			}
			catch (Exception ex)
			{

			}

			return null;
		}
	}
}
