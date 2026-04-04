using System.Diagnostics.CodeAnalysis;
using ShopService.Core.Entities;

namespace ShopService.Contracts.Responses
{
	public class BasketResponse
	{
		public List<BasketItemResponse> Items { get; set; } = new();

		public BasketResponse(Basket basket)
		{
			foreach(var good in basket.Goods)
			{
				if (good.Good != null)
				{
					Items.Add(new BasketItemResponse(good));
				}
			}
		}
	}

	public class BasketItemResponse
	{
		public required Guid BasketItemId { get; set; }
		public required Guid GoodId { get; set; }
		public required string Title { get; set; }
		public required decimal Price { get; set; }
		public required	int Count { get; set; }
		public Guid? ImageId { get; set; }

		[SetsRequiredMembers]
		public BasketItemResponse(GoodInBasket goodInBasket)
		{
			BasketItemId = goodInBasket.Id;
			GoodId = goodInBasket.Good!.Id;
			Title = goodInBasket.Good!.Title;
			Price = goodInBasket.Good!.Price;
			Count = goodInBasket.Count;
			ImageId = goodInBasket.Good!.ImageId;
		}
	}
}
