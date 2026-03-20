using System.Linq.Expressions;
using AdministrativeService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdministrativeService.Database.Interfaces
{
	public interface IBaseRepository<TEntity> where TEntity : BaseEntity
	{
		void SetUser(User user);
		Task<bool> SetUser(Guid id);
		TEntity Create(TEntity entity);
		void Delete(TEntity entity);
		Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, int page = 1, int pageSize = 100);
		Task<TEntity?> GetByIdAsync(Guid id);
		Task<IList<TEntity>> GetByPage(int page = 1, int pageSize = 10);
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		TEntity Update(TEntity entity);
	}
}
