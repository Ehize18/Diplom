namespace ShopService.Contracts.Requests
{
	public record AddGoodToBasketRequest(Guid GoodId, int Count = 1);
}
