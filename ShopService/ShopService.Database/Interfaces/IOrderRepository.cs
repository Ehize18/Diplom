using ShopService.Core.Entities;

namespace ShopService.Database.Interfaces
{
	public class UserOrderStats
	{
		public Guid UserId { get; set; }
		public int OrdersCount { get; set; }
		public decimal TotalSum { get; set; }
		public DateTime? LastOrderDate { get; set; }
	}

	public interface IOrderRepository : IBaseRepository<Order>
	{
		Task<(int Count, decimal TotalSum)> GetUserOrderStats(Guid userId, CancellationToken cancellationToken = default);
		Task<Dictionary<Guid, UserOrderStats>> GetUsersOrderStats(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
	}
}
