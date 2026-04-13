using ShopService.Core.Entities;

namespace ShopService.Application.DTO
{
	public class GoodWithSold : Good
	{
		public int SoldCount { get; set; }
	}
}
