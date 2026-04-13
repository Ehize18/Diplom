using ShopService.Core.Entities;

namespace ShopService.Application.DTO
{
	public class UserWithStats : User
	{
		public int OrdersCount { get; set; }
		public decimal TotalSum { get; set; }
		public DateTime? LastOrderDate { get; set; }
	}
}
