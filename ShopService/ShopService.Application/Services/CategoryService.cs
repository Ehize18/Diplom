using System.Linq.Expressions;
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

				var result = _categoryRepository.Create(category);
				await _categoryRepository.SaveChangesAsync(cancellationToken);
				return result;
			}
			catch (Exception ex)
			{
			}
			
			return null;
		}

		public async Task<DataGetResponse> HandleDataGetRequest(DataGet request)
		{
			var filterExp = BuildFilterExpression(request.Filter);
			try
			{
				var categories = await _categoryRepository.GetAsync(filterExp, request.OrderBy, request.IsAscending, request.Page, request.PageSize);
				return new DataGetResponse
				{
					Results = categories.ToArray(),
					IsSuccess = true,
					Error = string.Empty
				};
			}
			catch (Exception ex)
			{
				return new DataGetResponse
				{
					IsSuccess = false,
					Error = ex.Message
				};
			}
		}

		private Expression<Func<GoodCategory, bool>>? BuildFilterExpression(Filter? filter)
		{
			if (filter == null)
			{
				return null;
			}
			var parameter = Expression.Parameter(typeof(GoodCategory), "x");
			var property = Expression.Property(parameter, filter.LeftExpression);
			Expression constant = Expression.Constant(filter.RightExpression);

			if (property.Type != constant.Type)
			{
				constant = Expression.Convert(constant, property.Type);
			}

			Expression body;

			switch (filter.FilterType)
			{
				case FilterType.Equal:
					body = Expression.Equal(property, constant);
					break;
				case FilterType.Like:
					body = Expression.Call(property, nameof(string.Contains), Type.EmptyTypes, constant);
					break;
				default:
					return null;
			}

			return Expression.Lambda<Func<GoodCategory, bool>>(body, parameter);
		}
	}
}
