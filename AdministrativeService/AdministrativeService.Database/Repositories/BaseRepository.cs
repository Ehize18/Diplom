using System.Linq.Expressions;
using AdministrativeService.Core.Entities;
using AdministrativeService.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdministrativeService.Database.Repositories
{
	/// <summary>
	/// Base repository.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
	{
		/// <summary>
		/// Database context.
		/// </summary>
		protected readonly AdminDbContext _context;

		public void SetUser(User user)
		{
			_context.SetUser(user);
		}

		public async Task<bool> SetUser(Guid id)
		{
			return await _context.SetUser(id);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="context">Database context</param>
		protected BaseRepository(AdminDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Adds entity to Change Tracker with EntityState.Added
		/// </summary>
		/// <param name="entity">Entity.</param>
		/// <returns>Entity.</returns>
		public virtual TEntity Create(TEntity entity)
		{
			var entry = _context.Set<TEntity>()
				.Add(entity);

			return entry.Entity;
		}

		/// <summary>
		/// Gets entity by id.
		/// </summary>
		/// <param name="id">Entity id.</param>
		/// <returns>Task with entity.</returns>
		public virtual async Task<TEntity?> GetByIdAsync(Guid id)
		{
			var entity = await _context.Set<TEntity>()
				.FirstOrDefaultAsync(x => x.Id == id);

			return entity;
		}

		/// <summary>
		/// Gets entities by predicate.
		/// </summary>
		/// <param name="predicate">Predicate.</param>
		/// <param name="page">Page number.</param>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Task with entity list.</returns>
		public virtual async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, int page = 1, int pageSize = 100)
		{
			var entities = await _context.Set<TEntity>()
				.Where(predicate)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return entities;
		}

		/// <summary>
		/// Gets entities with pagination.
		/// </summary>
		/// <param name="page">Page number.</param>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Task with entity list.</returns>
		/// <exception cref="ArgumentException"></exception>
		public virtual async Task<IList<TEntity>> GetByPage(int page = 1, int pageSize = 10)
		{
			if (page < 1 || pageSize < 1)
			{
				throw new ArgumentException("Page and PageSize must be greater then zero.");
			}

			var entities = await _context.Set<TEntity>()
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return entities;
		}

		/// <summary>
		/// Track entity with EntityState.Modified.
		/// </summary>
		/// <param name="entity">Entity.</param>
		/// <returns>Entity.</returns>
		public virtual TEntity Update(TEntity entity)
		{
			var entry = _context.Set<TEntity>()
				.Update(entity);

			return entry.Entity;
		}

		/// <summary>
		/// Track entity with EntityState.Deleted.
		/// </summary>
		/// <param name="entity">Сущность.</param>
		public virtual void Delete(TEntity entity)
		{
			_context.Set<TEntity>().Remove(entity);
		}

		/// <summary>
		/// Save changes.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns>Task with number of changed rows.</returns>
		public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await _context.SaveChangesAsync(cancellationToken);
		}
	}
}
