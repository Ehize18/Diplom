using Shared.RabbitMQ.Contracts;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Application.Services
{
	public class CategoryService
	{
		private readonly IBaseRepository<GoodCategory> _categoryRepository;
		private readonly IGoodRepository _goodRepository;

		public CategoryService(IBaseRepository<GoodCategory> categoryRepository, IGoodRepository goodRepository)
		{
			_categoryRepository = categoryRepository;
			_goodRepository = goodRepository;
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
					Id = update.CategoryId == null ? Guid.NewGuid() : (Guid)update.CategoryId,
					Title = update.CategoryTitle!,
					Description = update.CategoryDescription != null ? update.CategoryDescription : string.Empty,
					ParentCategoryId = update.ParentCategoryId,
					IsActive = update.IsActive != null ? update.IsActive.Value : false,
					ImageId = update.ImageId
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

		public async Task<GoodCategory?> DeleteCategoryAsync(UpdateCategory update, CancellationToken cancellationToken = default)
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

				// 1. У дочерних категорий сбрасываем ParentCategoryId
				var childCategories = await _categoryRepository.GetAsync(x => x.ParentCategoryId == category.Id, null, false, 1, 1000);
				foreach (var child in childCategories)
				{
					child.ParentCategoryId = null;
					_categoryRepository.Update(child);
				}

				// 2. Если есть товары — создаём категорию "Товары без категорий + дата" и перемещаем товары
				var goods = await _goodRepository.GetAsync(x => x.CategoryId == category.Id, null, false, 1, 1);
				if (goods.Count > 0)
				{
					var today = DateTime.Now.ToString("dd.MM.yyyy");
					var noCategoryTitle = $"Товары без категорий {today}";

					var noCategory = new GoodCategory
					{
						Title = noCategoryTitle,
						Description = string.Empty,
						ParentCategoryId = null,
						IsActive = false,
						ImageId = null
					};
					await _categoryRepository.SetUser(update.UpdatedById);
					var createdCategory = _categoryRepository.Create(noCategory);
					await _categoryRepository.SaveChangesAsync(cancellationToken);

					// Перемещаем все товары
					var allGoods = await _goodRepository.GetAsync(x => x.CategoryId == category.Id, null, false, 1, 10000);
					foreach (var good in allGoods)
					{
						good.CategoryId = createdCategory.Id;
						_goodRepository.Update(good);
					}
				}

				// 3. Удаляем категорию
				_categoryRepository.Delete(category);
				await _categoryRepository.SaveChangesAsync(cancellationToken);

				return category;
			}
			catch (Exception)
			{
			}

			return null;
		}
	}
}
