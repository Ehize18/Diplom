using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Application.Services
{
	public class BasketService
	{
		private readonly IBasketRepository _basketRepository;

		public BasketService(IBasketRepository basketRepository)
		{
			_basketRepository = basketRepository;
		}

		public async Task<(bool, string)> AddGoodToCurrentBasket(User user, Guid goodId, int count, CancellationToken cancellationToken = default)
		{
			if (count <= 0)
			{
				count = 1;
			}

			_basketRepository.SetUser(user);

			var isSuccess = false;
			var error = string.Empty;

			(isSuccess, error) = await _basketRepository.AddGoodToCurrentBasket(goodId, count, cancellationToken);

			if (isSuccess)
			{
				try
				{
					await _basketRepository.SaveChangesAsync(cancellationToken);
				}
				catch (Exception ex)
				{
					isSuccess = false;
					error = "db_error";
				}
			}
			return (isSuccess, error);
		}

		public async Task<Basket> GetCurrentBasketWithItems(User user, CancellationToken cancellationToken = default)
		{
			_basketRepository.SetUser(user);
			var basket = await _basketRepository.GetCurrentBasket(true, cancellationToken);
			if (basket == null)
			{
				basket = new Basket();
				basket.UserId = user.Id;
				basket.IsCurrent = true;
				_basketRepository.Create(basket);
				await _basketRepository.SaveChangesAsync(cancellationToken);
			}

			return basket;
		}

		public async Task<int> ChangeBasketItemCount(User user, Guid basketItemId, int count, CancellationToken cancellationToken = default)
		{
			if (count <= 0)
			{
				count = 1;
			}

			_basketRepository.SetUser(user);
			return await _basketRepository.ChangeItemCountByCurrentUser(basketItemId, count, cancellationToken);
		}

		public async Task<int> DeleteBasketItem(User user, Guid basketItemId, CancellationToken cancellationToken = default)
		{
			_basketRepository.SetUser(user);
			return await _basketRepository.DeleteItemFromBasketByCurrentUser(basketItemId, cancellationToken);
		}
	}
}
