using System.Linq.Expressions;
using AdministrativeService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdministrativeService.Database.Repositories
{
	public class ShopRepository : BaseRepository<Shop>
	{
		public ShopRepository(AdminDbContext context) : base(context)
		{
		}
	}
}
