using AdministrativeService.Core.Entities;

namespace AdministrativeService.Database.Repositories
{
	public class ShopRepository : BaseRepository<Shop>
	{
		public ShopRepository(AdminDbContext context) : base(context)
		{
		}
	}
}
