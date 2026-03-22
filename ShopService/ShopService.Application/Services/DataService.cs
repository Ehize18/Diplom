using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Shared.RabbitMQ.Contracts;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Application.Services
{
	public class DataService
	{
		public DataService()
		{
			
		}

		public async Task<DataGetResponse> GetData(DataGet request, IServiceScope serviceScope)
		{
			switch (request.Entity)
			{
				case DataGetEntity.Category:
					return await GetData<GoodCategory>(request, serviceScope);
				case DataGetEntity.Good:
					return await GetData<Good>(request, serviceScope);
				default:
					return new DataGetResponse
					{
						IsSuccess = false,
						Error = "unavailable_entity"
					};
			}
		}

		private async Task<DataGetResponse> GetData<T>(DataGet request, IServiceScope serviceScope) where T : BaseEntity
		{
			var filterExp = BuildFilterExpression<T>(request.Filter);
			var repository = serviceScope.ServiceProvider.GetRequiredService<IBaseRepository<T>>();

			try
			{
				var categories = await repository.GetAsync(filterExp, request.OrderBy, request.IsAscending, request.Page, request.PageSize);
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

		private Expression<Func<T, bool>>? BuildFilterExpression<T>(Filter? filter) where T : BaseEntity
		{
			if (filter == null)
			{
				return null;
			}
			var parameter = Expression.Parameter(typeof(T), "x");
			var property = Expression.Property(parameter, filter.LeftExpression);
			Expression constant = Expression.Constant(filter.RightExpression);

			if (property.Type != constant.Type)
			{
				if (property.Type == typeof(Guid))
				{
					constant = Expression.Constant(Guid.Parse(filter.RightExpression));
				}
				else if (property.Type == typeof(Guid?))
				{
					Guid? guidValue = null;
					if (Guid.TryParse(filter.RightExpression, out var parsed)) 
					{
						guidValue = parsed;
					}
					constant = Expression.Constant((Guid?)guidValue);
					constant = Expression.Convert(constant, typeof(Guid?));
				}
				else
				{
					constant = Expression.Convert(constant, property.Type);
				}
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
			return Expression.Lambda<Func<T, bool>>(body, parameter);
		}
	}
}
