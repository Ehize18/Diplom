using System.Linq.Expressions;
using AdministrativeService.Core.Entities;

namespace AdministrativeService.Database.Repositories
{
	public class UserRepository : BaseRepository<User>
	{
		public UserRepository(AdminDbContext context) : base(context)
		{
		}
	}
}
