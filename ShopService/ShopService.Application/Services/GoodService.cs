using Shared.RabbitMQ.Contracts;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Application.Services
{
	public class GoodService
	{
		private readonly IGoodRepository _goodRepository;

		public GoodService(IGoodRepository goodRepository)
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
					ImageId = update.ImageId,
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
				var good = await _goodRepository.GetByIdAsync((Guid)update.GoodId!);
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
				good.ImageId = update.ImageId;
				_goodRepository.Update(good);
				await _goodRepository.SaveChangesAsync(cancellationToken);
				return good;
			}
			catch (Exception ex)
			{

			}

			return null;
		}

		public async Task<IEnumerable<Good?>> GetGoodsByCategory(Guid categoryId, bool isActual, CancellationToken cancellationToken = default)
		{
			return await _goodRepository.GetGoodsByCategory(categoryId, isActual, cancellationToken);
		}

		public async Task<IEnumerable<Good>> GetGoods(int page, int pageSize, CancellationToken cancellationToken = default)
		{
			return await _goodRepository.GetAsync(null, "CreatedAt", false, page, pageSize);
		}

		public async Task<Good?> GetGoodById(Guid id, CancellationToken cancellationToken = default)
		{
			return await _goodRepository.GetByIdAsync(id);
		}
	}
}
