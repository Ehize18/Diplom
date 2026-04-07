using AdministrativeService.Core.Entities;
using AdministrativeService.Database.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shared.RabbitMQ;
using Shared.RabbitMQ.Contracts;

namespace AdministrativeService.Application.Services
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
			Bus.AdminEvents.SHOP_CREATED, Bus.AdminEvents.SHOP_UPDATED,
			Bus.AdminEvents.METHOD_UPDATED,
			Bus.ShopEvents.CATEGORY_UPDATED, Bus.ShopEvents.GOOD_UPDATED,
			Bus.ShopEvents.PROPERTY_UPDATED, Bus.ShopEvents.PROPERTY_VALUE_UPDATED,
			Bus.ShopEvents.ORDER_UPDATED,
			Bus.DataBus.DATA_GET_RESPONSE, Bus.DataBus.GET_SHOP_BY_VK
		};

		protected override Dictionary<string, Dictionary<string, string>> QueueBinds => new Dictionary<string, Dictionary<string, string>>
		{
			{
				Bus.AdminEvents.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.AdminEvents.SHOP_CREATED, Bus.AdminEvents.SHOP_CREATED },
					{ Bus.AdminEvents.SHOP_UPDATED, Bus.AdminEvents.SHOP_UPDATED },
					{ Bus.AdminEvents.METHOD_UPDATED, Bus.AdminEvents.METHOD_UPDATED }
				}
			},
			{
				Bus.ShopEvents.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.ShopEvents.CATEGORY_UPDATED, Bus.ShopEvents.CATEGORY_UPDATED },
					{ Bus.ShopEvents.GOOD_UPDATED, Bus.ShopEvents.GOOD_UPDATED },
					{ Bus.ShopEvents.PROPERTY_UPDATED, Bus.ShopEvents.PROPERTY_UPDATED },
					{ Bus.ShopEvents.PROPERTY_VALUE_UPDATED, Bus.ShopEvents.PROPERTY_VALUE_UPDATED },
					{ Bus.ShopEvents.ORDER_UPDATED, Bus.ShopEvents.ORDER_UPDATED }
				}
			},
			{
				Bus.DataBus.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.DataBus.DATA_GET_RESPONSE, Bus.DataBus.DATA_GET_RESPONSE },
					{ Bus.DataBus.GET_SHOP_BY_VK, Bus.DataBus.GET_SHOP_BY_VK }
				}
			}
		};

		protected override int DefaultTimeout => 120 * 1000;


		protected override async Task InitConsumers(CancellationToken cancellationToken = default)
		{
			await AddReadConsumer<ShopCreated>(Bus.AdminEvents.SHOP_CREATED, cancellationToken);
			await AddReadConsumer<MethodUpdated>(Bus.AdminEvents.METHOD_UPDATED, cancellationToken);
			await AddReadConsumer<CategoryUpdated>(Bus.ShopEvents.CATEGORY_UPDATED, cancellationToken);
			await AddReadConsumer<DataGetResponse>(Bus.DataBus.DATA_GET_RESPONSE, cancellationToken);
			await AddReadConsumer<GoodUpdated>(Bus.ShopEvents.GOOD_UPDATED, cancellationToken);
			await AddReadConsumer<PropertyUpdated>(Bus.ShopEvents.PROPERTY_UPDATED, cancellationToken);
			await AddReadConsumer<PropertyValueUpdated>(Bus.ShopEvents.PROPERTY_VALUE_UPDATED, cancellationToken);
			await AddReadConsumer<OrderUpdated>(Bus.ShopEvents.ORDER_UPDATED, cancellationToken);
			await InitGetShopByVkConsumer(cancellationToken);
		}

		private async Task InitGetShopByVkConsumer(CancellationToken cancellationToken = default)
		{
			await AddConsumer<GetShopByVk>(Bus.DataBus.GET_SHOP_BY_VK, async (model, ea, message) =>
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<Shop>>();
					var shop = (await repository.GetAsync(x => x.VkGroupId == message.VkGroupId)).FirstOrDefault();
					var response = new GetShopByVkResponse
					{
						ShopId = shop?.Id
					};
					var properties = CreateProperties();
					properties.CorrelationId = ea.BasicProperties.CorrelationId;
					await PublishMessage(properties, response, Bus.DataBus.EXCHANGE, ea.BasicProperties.ReplyTo);
				}
			}, cancellationToken);
		}

		public async Task<bool> PublishCreateShopMessage<TProperties>(TProperties properties, CreateShop body, CancellationToken cancellationToken = default)
			where TProperties : IReadOnlyBasicProperties, IAmqpHeader
		{
			return await PublishMessage(properties, body, Bus.AdminEvents.EXCHANGE, Bus.AdminEvents.SHOP_CREATE, cancellationToken);
		}

		public async Task<bool> PublishUpdateCategoryMessage<TProperties>(TProperties properties, UpdateCategory body, CancellationToken cancellationToken = default)
			where TProperties : IReadOnlyBasicProperties, IAmqpHeader
		{
			return await PublishMessage(properties, body, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.CATEGORY_UPDATE, cancellationToken);
		}

		public async Task<bool> PublishUpdateGoodMessage<TProperties>(TProperties properties, UpdateGood body, CancellationToken cancellationToken = default)
			where TProperties : IReadOnlyBasicProperties, IAmqpHeader
		{
			return await PublishMessage(properties, body, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.GOOD_UPDATE, cancellationToken);
		}

		public async Task<bool> PublishUpdateOrderMessage<TProperties>(TProperties properties, UpdateOrder body, CancellationToken cancellationToken = default)
			where TProperties : IReadOnlyBasicProperties, IAmqpHeader
		{
			return await PublishMessage(properties, body, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.ORDER_UPDATE, cancellationToken);
		}

		public async Task<bool> PublishDataGetMessage<TProperties>(TProperties properties, DataGet body, CancellationToken cancellationToken = default)
			where TProperties : IReadOnlyBasicProperties, IAmqpHeader
		{
			return await PublishMessage(properties, body, Bus.DataBus.EXCHANGE, Bus.DataBus.DATA_GET, cancellationToken);
		}

		public async Task<PropertyUpdated?> UpdateProperty(UpdateProperty body, CancellationToken cancellationToken = default)
		{
			var messageId = Guid.NewGuid();
			var resultTask = GetAnswerAsync<PropertyUpdated>(messageId, cancellationToken);

			var properties = CreateProperties();
			properties.CorrelationId = messageId.ToString();

			await PublishMessage(properties, body, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.PROPERTY_UPDATE, cancellationToken);

			return await resultTask;
		}

		public async Task<PropertyValueUpdated?> UpdatePropertyValue(UpdatePropertyValue body, CancellationToken cancellationToken = default)
		{
			var messageId = Guid.NewGuid();
			var resultTask = GetAnswerAsync<PropertyValueUpdated>(messageId, cancellationToken);

			var properties = CreateProperties();
			properties.CorrelationId = messageId.ToString();

			await PublishMessage(properties, body, Bus.ShopEvents.EXCHANGE, Bus.ShopEvents.PROPERTY_VALUE_UPDATE, cancellationToken);

			return await resultTask;
		}

		public async Task<MethodUpdated?> UpdateMethod(UpdateMethod body, CancellationToken cancellationToken = default)
		{
			var messageId = Guid.NewGuid();
			var resultTask = GetAnswerAsync<MethodUpdated>(messageId, cancellationToken);

			var properties = CreateProperties();
			properties.CorrelationId = messageId.ToString();

			await PublishMessage(properties, body, Bus.AdminEvents.EXCHANGE, Bus.AdminEvents.METHOD_UPDATE, cancellationToken);

			return await resultTask;
		}
	}
}
