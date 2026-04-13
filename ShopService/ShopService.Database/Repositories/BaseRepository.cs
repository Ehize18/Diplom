
using System.Linq.Expressions;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ShopService.Database.Repositories
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
		protected readonly ShopDbContext _context;

		public void SetUser(User user)
		{
			_context.SetUser(user);
		}

		public async Task<bool> SetUser(Guid id)
		{
			return await _context.SetUser(id);
		}

		public async Task<bool> CheckOrCreateAdmin(Guid adminId)
		{
			var admin = await _context.User.FirstOrDefaultAsync(x => x.Id == adminId);

			if (admin == null)
			{
				_context.User.Add(new User
				{
					Id = adminId,
					Username = "Admin",
					IsAdmin = true
				});
				return true;
			}
			return admin.IsAdmin;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="context">Database context</param>
		protected BaseRepository(ShopDbContext context)
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
		public virtual async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate, string? orderBy, bool isAscending = false, int page = 1, int pageSize = 100)
		{
			IQueryable<TEntity> entities = _context.Set<TEntity>();

			if (predicate != null)
			{
				entities = entities.Where(predicate);
			}

			if (!string.IsNullOrWhiteSpace(orderBy))
			{
				if (isAscending)
				{
					entities = entities.OrderBy(GetOrderByExpression(orderBy));
				}
				else
				{
					entities = entities.OrderByDescending(GetOrderByExpression(orderBy));
				}
			}

			if (page != null && page > 0 && pageSize != null && pageSize > 0)
			{
				entities = entities
					.Skip((page - 1) * pageSize)
					.Take(pageSize);
			}

			return await entities.ToListAsync();
		}

		public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
		{
			IQueryable<TEntity> query = _context.Set<TEntity>();
			if (predicate != null)
			{
				query = query.Where(predicate);
			}
			return await query.CountAsync(cancellationToken);
		}

		protected abstract Expression<Func<TEntity, object>> GetOrderByExpression(string orderBy);

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
