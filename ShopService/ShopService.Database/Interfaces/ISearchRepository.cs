using ShopService.Core.Entities;

namespace ShopService.Database.Interfaces
{
	public interface ISearchRepository
	{
		Task<List<CategoryOrGoodSearch>> SearchAsync(string query, int limit = 20, double similarityThreshold = 0.2, CancellationToken cancellationToken = default);
		Task<List<CategoryOrGoodSearch>> SearchSimpleAsync(string query, int limit = 20, CancellationToken cancellationToken = default);
	}
}