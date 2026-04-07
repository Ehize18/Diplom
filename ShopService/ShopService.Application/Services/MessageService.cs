using Microsoft.Extensions.DependencyInjection;
using Shared.RabbitMQ;
using Shared.RabbitMQ.Contracts;
using ShopService.Core;
using ShopService.Core.Entities;

namespace ShopService.Application.Services
{
	public class MessageService : RabbitMQService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly Guid _serviceId;

		private string GetShopByVkResponseQueue => $"{Bus.DataBus.GET_SHOP_BY_VK_RESPONSE}.{this._serviceId}";

		public MessageService(IServiceProvider serviceProvider,
			RabbitMQConsumer consumer, RabbitMQPublisher publisher) : base(consumer, publisher)
		{
			_serviceProvider = serviceProvider;
			_serviceId = Guid.NewGuid();
		}

		protected override string[] Exchanges => new[] { Bus.AdminEvents.EXCHANGE, Bus.ShopEvents.EXCHANGE, Bus.DataBus.EXCHANGE };

		protected override string[] Queues => new[]
		{
			Bus.AdminEvents.SHOP_CREATE, Bus.AdminEvents.SHOP_UPDATE,
			Bus.AdminEvents.METHOD_UPDATE,
			Bus.ShopEvents.CATEGORY_UPDATE, Bus.ShopEvents.GOOD_UPDATE,
			Bus.ShopEvents.PROPERTY_UPDATE, Bus.ShopEvents.PROPERTY_VALUE_UPDATE,
			Bus.ShopEvents.ORDER_UPDATE,
			Bus.DataBus.DATA_GET, GetShopByVkResponseQueue
		};

		protected override Dictionary<string, Dictionary<string, string>> QueueBinds => new Dictionary<string, Dictionary<string, string>>
		{
			{
				Bus.AdminEvents.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.AdminEvents.SHOP_CREATE, Bus.AdminEvents.SHOP_CREATE },
					{ Bus.AdminEvents.SHOP_UPDATE, Bus.AdminEvents.SHOP_UPDATE },
					{ Bus.AdminEvents.METHOD_UPDATE, Bus.AdminEvents.METHOD_UPDATE }
				}
			},
			{
				Bus.ShopEvents.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.ShopEvents.CATEGORY_UPDATE, Bus.ShopEvents.CATEGORY_UPDATE },
					{ Bus.ShopEvents.GOOD_UPDATE, Bus.ShopEvents.GOOD_UPDATE },
					{ Bus.ShopEvents.PROPERTY_UPDATE, Bus.ShopEvents.PROPERTY_UPDATE },
					{ Bus.ShopEvents.PROPERTY_VALUE_UPDATE, Bus.ShopEvents.PROPERTY_VALUE_UPDATE },
					{ Bus.ShopEvents.ORDER_UPDATE, Bus.ShopEvents.ORDER_UPDATE }
				}
			},
			{
				Bus.DataBus.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.DataBus.DATA_GET, Bus.DataBus.DATA_GET },
					{ GetShopByVkResponseQueue, GetShopByVkResponseQueue }
				}
			}
		};

		protected override int DefaultTimeout => 120 * 1000;

		protected override async Task InitConsumers(CancellationToken cancellationToken = default)
		{
			await InitShopCreateConsumer(cancellationToken);
			await InitMethodUpdateConsumer(cancellationToken);
			await InitCategoryUpdateConsumer(cancellationToken);
			await InitGoodUpdateConsumer(cancellationToken);
			await InitDataGetConsumer(cancellationToken);
			await InitPropertyUpdateConsumer(cancellationToken);
			await InitOrderUpdateConsumer(cancellationToken);
			await AddReadConsumer<GetShopByVkResponse>(GetShopByVkResponseQueue, cancellationToken);
		}

		private async Task InitMethodUpdateConsumer(CancellationToken cancellationToken = default)
		{
			await AddConsumer<UpdateMethod>(Bus.AdminEvents.METHOD_UPDATE, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = message.ShopId;
					var methodsService = scope.ServiceProvider.GetRequiredService<MethodsService>();
					MethodUpdated? methodUpdated = null;
					switch (message.UpdateType)
					{
						case UpdateType.Create:
							methodUpdated = await methodsService.CreateMethod(message);
							break;
					}

					if (methodUpdated == null)
					{
						methodUpdated = new MethodUpdated
						{
							MethodId = message.MethodId != null ? (Guid)message.MethodId : Guid.Empty,
							IsSuccess = false,
							Error = "unexpected_error"
						};
					}

					var properties = CreateProperties();
					properties.CorrelationId = ea.BasicProperties.CorrelationId;
					await PublishMessage(properties, methodUpdated, Bus.AdminEvents.EXCHANGE, Bus.AdminEvents.METHOD_UPDATED);
				}
			}, cancellationToken);
		}

		private async Task InitShopCreateConsumer(CancellationToken cancellationToken = default)
		{
			await AddConsumer<CreateShop>(Bus.AdminEvents.SHOP_CREATE, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = message.ShopId;
					var dbMigrator = scope.ServiceProvider.GetRequiredService<DbMigrator>();
					var response = await dbMigrator.Migrate(message.ShopId);

					if (response != null)
					{
						var properties = CreateProperties();
						properties.CorrelationId = ea.BasicProperties.CorrelationId;
						await PublishMessage(properties, response, Bus.AdminEvents.EXCHANGE, Bus.AdminEvents.SHOP_CREATED);
					}
				}
			}, cancellationToken);
		}

		private async Task InitCategoryUpdateConsumer(CancellationToken cancellationToken = default)
		{
			await AddConsumer<UpdateCategory>(Bus.ShopEvents.CATEGORY_UPDATE, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = message.ShopId;
					var categoryService = scope.ServiceProvider.GetRequiredService<CategoryService>();
					GoodCategory? category = null;
					switch (message.UpdateType)
					{
						case UpdateType.Create:
							category = await categoryService.CreateCategoryAsync(message);
							break;
						case UpdateType.Update:
							category = await categoryService.UpdateCategoryAsync(message);
							break;
						default:
							break;
					}
					
					CategoryUpdated categoryUpdated;
					if (category != null)
					{
						categoryUpdated = new CategoryUpdated
						{
							CategoryId = category.Id,
							IsSuccess = true
						};
					}
					else
					{
						categoryUpdated = new CategoryUpdated
						{
							CategoryId = Guid.Empty,
							IsSuccess = false
						};
					}

					var properties = CreateProperties();
					properties.CorrelationId = ea.BasicProperties.CorrelationId;
					await PublishMessage(properties, categoryUpdated, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.CATEGORY_UPDATED);
				}
			}, cancellationToken);
		}

		private async Task InitGoodUpdateConsumer(CancellationToken cancellationToken = default)
		{
			await AddConsumer<UpdateGood>(Bus.ShopEvents.GOOD_UPDATE, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = message.ShopId;
					var categoryService = scope.ServiceProvider.GetRequiredService<GoodService>();
					Good? good = null;
					switch (message.UpdateType)
					{
						case UpdateType.Create:
							good = await categoryService.CreateGoodAsync(message);
							break;
						case UpdateType.Update:
							good = await categoryService.UpdateGoodAsync(message);
							break;
						default:
							break;
					}

					GoodUpdated goodUpdated;
					if (good != null)
					{
						goodUpdated = new GoodUpdated
						{
							GoodId = good.Id,
							IsSuccess = true
						};
					}
					else
					{
						goodUpdated = new GoodUpdated
						{
							GoodId = Guid.Empty,
							IsSuccess = false
						};
					}

					var properties = CreateProperties();
					properties.CorrelationId = ea.BasicProperties.CorrelationId;
					await PublishMessage(properties, goodUpdated, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.GOOD_UPDATED);
				}
			}, cancellationToken);
		}

		private async Task InitPropertyUpdateConsumer(CancellationToken cancellationToken = default)
		{
			await AddConsumer<UpdateProperty>(Bus.ShopEvents.PROPERTY_UPDATE, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = message.ShopId;
					var categoryService = scope.ServiceProvider.GetRequiredService<PropertyService>();
					GoodPropertyCategory? property = null;
					switch (message.UpdateType)
					{
						case UpdateType.Create:
							property = await categoryService.CreateGoodProperty(message);
							break;
						case UpdateType.Update:
							property = await categoryService.UpdateGoodProperty(message);
							break;
						default:
							break;
					}

					PropertyUpdated goodUpdated;
					if (property != null)
					{
						goodUpdated = new PropertyUpdated
						{
							PropertyId = property.Id,
							IsSuccess = true
						};
					}
					else
					{
						goodUpdated = new PropertyUpdated
						{
							PropertyId = Guid.Empty,
							IsSuccess = false
						};
					}

					var properties = CreateProperties();
					properties.CorrelationId = ea.BasicProperties.CorrelationId;
					await PublishMessage(properties, goodUpdated, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.PROPERTY_UPDATED);
				}
			}, cancellationToken);

			await AddConsumer<UpdatePropertyValue>(Bus.ShopEvents.PROPERTY_VALUE_UPDATE, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = message.ShopId;
					var categoryService = scope.ServiceProvider.GetRequiredService<PropertyService>();
					GoodProperty? propertyValue = null;
					switch (message.UpdateType)
					{
						case UpdateType.Create:
							propertyValue = await categoryService.CreateGoodPropertyValue(message);
							break;
						case UpdateType.Update:
							propertyValue = await categoryService.UpdateGoodPropertyValue(message);
							break;
						default:
							break;
					}

					PropertyValueUpdated goodUpdated;
					if (propertyValue != null)
					{
						goodUpdated = new PropertyValueUpdated
						{
							PropertyValueId = propertyValue.Id,
							IsSuccess = true
						};
					}
					else
					{
						goodUpdated = new PropertyValueUpdated
						{
							PropertyValueId = Guid.Empty,
							IsSuccess = false
						};
					}

					var properties = CreateProperties();
					properties.CorrelationId = ea.BasicProperties.CorrelationId;
					await PublishMessage(properties, goodUpdated, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.PROPERTY_VALUE_UPDATED);
				}
			}, cancellationToken);
		}

		private async Task InitOrderUpdateConsumer(CancellationToken cancellationToken = default)
		{
			await AddConsumer<UpdateOrder>(Bus.ShopEvents.ORDER_UPDATE, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = message.ShopId;
					var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();
					var success = await orderService.UpdateOrderStatus(message.OrderId, message.EntityType, message.StatusValue, cancellationToken);

					var orderUpdated = new OrderUpdated
					{
						OrderId = message.OrderId,
						IsSuccess = success,
						Error = success ? null : "unexpected_error"
					};

					var properties = CreateProperties();
					properties.CorrelationId = ea.BasicProperties.CorrelationId;
					await PublishMessage(properties, orderUpdated, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.ORDER_UPDATED);
				}
			}, cancellationToken);
		}

		private async Task InitDataGetConsumer(CancellationToken cancellationToken = default)
		{
			await AddConsumer<DataGet>(Bus.DataBus.DATA_GET, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = message.ShopId;
					var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
					var response = await dataService.GetData(message, scope);
					var properties = CreateProperties();
					properties.CorrelationId = ea.BasicProperties.CorrelationId;
					await PublishMessage(properties, response, Bus.DataBus.EXCHANGE, Bus.DataBus.DATA_GET_RESPONSE);
				}
			}, cancellationToken);
		}

		public async Task<GetShopByVkResponse?> GetShopByVk(long vkGroupId, CancellationToken cancellationToken = default)
		{
			var messageId = Guid.NewGuid();
			var resultTask = GetAnswerAsync<GetShopByVkResponse>(messageId);
			var message = new GetShopByVk { VkGroupId = vkGroupId };
			var properties = CreateProperties();
			properties.CorrelationId = messageId.ToString();
			properties.ReplyTo = GetShopByVkResponseQueue;
			await PublishMessage(properties, message, Bus.DataBus.EXCHANGE, Bus.DataBus.GET_SHOP_BY_VK);
			return await resultTask;
		}
	}
}
