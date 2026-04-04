using ShopService.Core.Entities;

namespace ShopService.Database.Interfaces
{
	public interface IBasketRepository : IBaseRepository<Basket>
	{
		Task<Basket?> GetCurrentBasket(bool withItems = false, CancellationToken cancellationToken = default);
		Task<(bool, string)> AddGoodToBasket(Guid basketId, Guid goodId, int count, CancellationToken cancellationToken = default);
		Task<(bool, string)> AddGoodToCurrentBasket(Guid goodId, int count, CancellationToken cancellationToken = default);
		Task<int> ChangeItemCountByCurrentUser(Guid basketItemId, int count, CancellationToken cancellationToken = default);
		Task<int> DeleteItemFromBasketByCurrentUser(Guid basketItemId, CancellationToken cancellationToken = default);
	}
}