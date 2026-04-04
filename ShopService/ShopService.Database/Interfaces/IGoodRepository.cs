using ShopService.Core.Entities;

namespace ShopService.Database.Interfaces
{
	public interface IGoodRepository : IBaseRepository<Good>
	{
		Task<IEnumerable<Good?>> GetGoodsByCategory(Guid categoryId, bool isActual, CancellationToken cancellationToken = default);
	}
}