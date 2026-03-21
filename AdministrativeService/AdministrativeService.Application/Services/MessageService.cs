using RabbitMQ.Client;
using Shared.RabbitMQ;
using Shared.RabbitMQ.Contracts;

namespace AdministrativeService.Application.Services
{
	public class MessageService : RabbitMQService
	{
		protected override string[] Exchanges => new[] { Bus.AdminEvents.EXCHANGE, Bus.ShopEvents.EXCHANGE, Bus.DataBus.EXCHANGE };

		protected override string[] Queues => new[] 
		{
			Bus.AdminEvents.SHOP_CREATED, Bus.AdminEvents.SHOP_UPDATED,
			Bus.ShopEvents.CATEGORY_UPDATED, Bus.DataBus.DATA_GET_RESPONSE
		};

		protected override Dictionary<string, Dictionary<string, string>> QueueBinds => new Dictionary<string, Dictionary<string, string>>
		{
			{
				Bus.AdminEvents.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.AdminEvents.SHOP_CREATED, Bus.AdminEvents.SHOP_CREATED },
					{ Bus.AdminEvents.SHOP_UPDATED, Bus.AdminEvents.SHOP_UPDATED }
				}
			},
			{
				Bus.ShopEvents.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.ShopEvents.CATEGORY_UPDATED, Bus.ShopEvents.CATEGORY_UPDATED },
				}
			},
			{
				Bus.DataBus.EXCHANGE,
				new Dictionary<string, string>
				{
					{ Bus.DataBus.DATA_GET_RESPONSE, Bus.DataBus.DATA_GET_RESPONSE },
				}
			}
		};

		protected override int DefaultTimeout => 120 * 1000;

		public MessageService(RabbitMQConsumer consumer, RabbitMQPublisher publisher) : base(consumer, publisher)
		{
		}

		protected override async Task InitConsumers(CancellationToken cancellationToken = default)
		{
			await AddReadConsumer<ShopCreated>(Bus.AdminEvents.SHOP_CREATED, cancellationToken);
			await AddReadConsumer<CategoryUpdated>(Bus.ShopEvents.CATEGORY_UPDATED, cancellationToken);
			await AddReadConsumer<DataGetResponse>(Bus.DataBus.DATA_GET_RESPONSE, cancellationToken);
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

		public async Task<bool> PublishDataGetMessage<TProperties>(TProperties properties, DataGet body, CancellationToken cancellationToken = default)
			where TProperties : IReadOnlyBasicProperties, IAmqpHeader
		{
			return await PublishMessage(properties, body, Bus.DataBus.EXCHANGE, Bus.DataBus.DATA_GET, cancellationToken);
		}
	}
}
