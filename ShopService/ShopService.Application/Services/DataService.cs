using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Shared.RabbitMQ.Contracts;
using ShopService.Application.DTO;
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
					return await GetGoodsWithSoldCount(request, serviceScope);
				case DataGetEntity.Property:
					return await GetData<GoodPropertyCategory>(request, serviceScope);
				case DataGetEntity.Order:
					return await GetData<Order>(request, serviceScope);
				case DataGetEntity.User:
					return await GetUsersWithStats(request, serviceScope);
				case DataGetEntity.ShopStatistics:
					return await GetShopStatistics(request, serviceScope);
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
				else if (property.Type == typeof(bool))
				{
					constant = Expression.Constant(bool.Parse(filter.RightExpression));
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

		private async Task<DataGetResponse> GetGoodsWithSoldCount(DataGet request, IServiceScope serviceScope)
		{
			var filterExp = BuildFilterExpression<Good>(request.Filter);
			var goodRepository = serviceScope.ServiceProvider.GetRequiredService<IBaseRepository<Good>>();
			var orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();

			try
			{
				var goods = await goodRepository.GetAsync(filterExp, request.OrderBy, request.IsAscending, request.Page, request.PageSize);

				var goodsSoldCount = await orderRepository.GetGoodsSoldCount();

				var goodsWithSold = goods.Select(good =>
				{
					var soldCount = goodsSoldCount.TryGetValue(good.Id, out var count) ? count : 0;
					return new GoodWithSold
					{
						Id = good.Id,
						Title = good.Title,
						Description = good.Description,
						Count = good.Count,
						Price = good.Price,
						OldPrice = good.OldPrice,
						CategoryId = good.CategoryId,
						ImageId = good.ImageId,
						IsDeleted = good.IsDeleted,
						CreatedAt = good.CreatedAt,
						UpdatedAt = good.UpdatedAt,
						CreatedById = good.CreatedById,
						UpdatedById = good.UpdatedById,
						SoldCount = soldCount
					};
				}).ToList();

				return new DataGetResponse
				{
					Results = goodsWithSold.ToArray(),
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

		private async Task<DataGetResponse> GetUsersWithStats(DataGet request, IServiceScope serviceScope)
		{
			var filterExp = BuildFilterExpression<User>(request.Filter);
			var userRepository = serviceScope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();
			var orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();

			try
			{
				var users = await userRepository.GetAsync(filterExp, request.OrderBy, request.IsAscending, request.Page, request.PageSize);

				var userIds = users.Select(u => u.Id).ToList();
				var statsDict = await orderRepository.GetUsersOrderStats(userIds);

				var usersWithStats = users.Select(user =>
				{
					var stats = statsDict.TryGetValue(user.Id, out var s) ? s : new UserOrderStats();
					return new UserWithStats
					{
						Id = user.Id,
						CreatedAt = user.CreatedAt,
						UpdatedAt = user.UpdatedAt,
						CreatedById = user.CreatedById,
						UpdatedById = user.UpdatedById,
						Username = user.Username,
						VkId = user.VkId,
						IsAdmin = user.IsAdmin,
						OrdersCount = stats.OrdersCount,
						TotalSum = stats.TotalSum,
						LastOrderDate = stats.LastOrderDate
					};
				}).ToList();

				return new DataGetResponse
				{
					Results = usersWithStats.ToArray(),
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
		private async Task<DataGetResponse> GetShopStatistics(DataGet request, IServiceScope serviceScope)
		{
			var categoryRepository = serviceScope.ServiceProvider.GetRequiredService<IBaseRepository<GoodCategory>>();
			var goodRepository = serviceScope.ServiceProvider.GetRequiredService<IBaseRepository<Good>>();
			var orderRepository = serviceScope.ServiceProvider.GetRequiredService<IOrderRepository>();
			var userRepository = serviceScope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

			try
			{
				var clientsCount = await userRepository.CountAsync(x => x.IsAdmin == false);
				var ordersCount = await orderRepository.CountAsync(null);
				var categoriesCount = await categoryRepository.CountAsync(null);
				var goodsCount = await goodRepository.CountAsync(null);

				var stats = new ShopStatistics
				{
					ClientsCount = clientsCount,
					OrdersCount = ordersCount,
					CategoriesCount = categoriesCount,
					GoodsCount = goodsCount
				};

				return new DataGetResponse
				{
					Results = new[] { stats },
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
	}
}
