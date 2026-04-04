using ShopService.Core.Entities;

namespace ShopService.Database.Interfaces
{
	public interface IGoodPropertyRepository : IBaseRepository<GoodPropertyCategory>
	{
		Task<GoodPropertyCategory?> GetGoodProperty(Guid id, bool withValues = false, CancellationToken cancellationToken = default);
		Task<List<GoodProperty>> GetGoodPropertyValues(Guid propertyId, CancellationToken cancellationToken = default);
		GoodProperty Create(GoodProperty propertyValue);
		GoodProperty Update(GoodProperty propertyValue);
		Task<GoodProperty?> GetPropertyValueById(Guid id, CancellationToken cancellationToken = default);
	}
}