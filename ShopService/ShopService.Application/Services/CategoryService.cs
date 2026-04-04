using Shared.RabbitMQ.Contracts;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Application.Services
{
	public class CategoryService
	{
		private readonly IBaseRepository<GoodCategory> _categoryRepository;

		public CategoryService(IBaseRepository<GoodCategory> categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		public async Task<GoodCategory?> CreateCategoryAsync(UpdateCategory update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _categoryRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return null;
			}

			try
			{
				var category = new GoodCategory
				{
					Title = update.CategoryTitle!,
					Description = update.CategoryDescription != null ? update.CategoryDescription : string.Empty,
					ParentCategoryId = update.ParentCategoryId,
					IsActive = update.IsActive != null ? update.IsActive.Value : false,
				};
				await _categoryRepository.SetUser(update.UpdatedById);
				var result = _categoryRepository.Create(category);
				await _categoryRepository.SaveChangesAsync(cancellationToken);
				return result;
			}
			catch (Exception ex)
			{
			}
			
			return null;
		}

		public async Task<GoodCategory?> UpdateCategoryAsync(UpdateCategory update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _categoryRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return null;
			}

			try
			{
				await _categoryRepository.SetUser(update.UpdatedById);
				var category = await _categoryRepository.GetByIdAsync((Guid)update.CategoryId!);
				if (category == null)
				{
					return null;
				}
				category.Title = update.CategoryTitle;
				category.Description = update.CategoryDescription;
				category.ParentCategoryId = update.ParentCategoryId;
				category.IsActive = (bool)update.IsActive;
				category.ImageId = update.ImageId;
				_categoryRepository.Update(category);
				await _categoryRepository.SaveChangesAsync(cancellationToken);
				return category;
			}
			catch (Exception ex)
			{

			}

			return null;
		}

		public async Task<List<GoodCategory>> GetCategoriesByParent(Guid? parentCategoryId, CancellationToken cancellationToken = default)
		{
			return await _categoryRepository.GetAsync(x => x.ParentCategoryId == parentCategoryId, "Title", true, 0, 0);
		}

		public async Task<GoodCategory?> GetCategoryById(Guid categoryId, bool withChilds, CancellationToken cancellationToken = default)
		{
			var category = await _categoryRepository.GetByIdAsync(categoryId);

			if (category == null)
			{
				return category;
			}

			var childs = await GetCategoriesByParent(category.Id, cancellationToken);

			category.ChildCategories = childs;

			return category;
		}
	}
}
