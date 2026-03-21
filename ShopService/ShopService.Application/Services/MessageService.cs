using Microsoft.Extensions.DependencyInjection;
using Shared.RabbitMQ;
using Shared.RabbitMQ.Contracts;
using ShopService.Core;

namespace ShopService.Application.Services
{
	public class MessageService : RabbitMQService
	{
		private readonly IServiceProvider _serviceProvider;

		public MessageService(IServiceProvider serviceProvider,
			RabbitMQConsumer consumer, RabbitMQPublisher publisher) : base(consumer, publisher)
		{
			_serviceProvider = serviceProvider;
		}

		protected override string[] Exchanges => new[] { Bus.AdminEvents.EXCHANGE, Bus.ShopEvents.EXCHANGE, Bus.DataBus.EXCHANGE };

		protected override string[] Queues => new[]
		{
			Bus.AdminEvents.SHOP_CREATE, Bus.AdminEvents.SHOP_UPDATE,
			Bus.ShopEvents.CATEGORY_UPDATE, Bus.DataBus.DATA_GET
		};

		protected override Dictionary<string, Dictionary<string, string>> QueueBinds => new Dictionary<string, Dictionary<string, string>>
		{
			{
				Bus.AdminEvents.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.AdminEvents.SHOP_CREATE, Bus.AdminEvents.SHOP_CREATE },
					{ Bus.AdminEvents.SHOP_UPDATE, Bus.AdminEvents.SHOP_UPDATE }
				}
			},
			{
				Bus.ShopEvents.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.ShopEvents.CATEGORY_UPDATE, Bus.ShopEvents.CATEGORY_UPDATE }
				}
			},
			{
				Bus.DataBus.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.DataBus.DATA_GET, Bus.DataBus.DATA_GET }
				}
			}
		};

		protected override int DefaultTimeout => 120 * 1000;

		protected override async Task InitConsumers(CancellationToken cancellationToken = default)
		{
			await InitShopCreateConsumer(cancellationToken);
			await InitCategoryUpdateConsumer(cancellationToken);
			await InitDataGetConsumer(cancellationToken);
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
					var category = await categoryService.CreateCategoryAsync(message);
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

		private async Task InitDataGetConsumer(CancellationToken cancellationToken = default)
		{
			await AddConsumer<DataGet>(Bus.DataBus.DATA_GET, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = message.ShopId;
					var response = await GetDataResponse(message, scope);
					var properties = CreateProperties();
					properties.CorrelationId = ea.BasicProperties.CorrelationId;
					await PublishMessage(properties, response, Bus.DataBus.EXCHANGE, Bus.DataBus.DATA_GET_RESPONSE);
				}
			}, cancellationToken);
		}

		private async Task<DataGetResponse> GetDataResponse(DataGet request, IServiceScope scope)
		{
			DataGetResponse response = new DataGetResponse
			{
				IsSuccess = false,
				Error = "unexpected_error"
			};
			switch (request.Entity)
			{
				case DataGetEntity.Category:
					var categoryService = scope.ServiceProvider.GetRequiredService<CategoryService>();
					response = await categoryService.HandleDataGetRequest(request);
					break;
				default:
					break;

			}
			return response;
		}
	}
}
