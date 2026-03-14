using System.Collections.Concurrent;
using AdministrativeService.Core.Entities;
using AdministrativeService.Database.Interfaces;
using Shared.RabbitMQ;
using Shared.RabbitMQ.Contracts;

namespace AdministrativeService.Application.Services
{
	public class ShopService
	{
		private readonly RabbitMQService _rabbit;
		private readonly IBaseRepository<Shop> _shopRepository;
		private readonly ConcurrentDictionary<Guid, MigrateDbResponse> _migrateResponses = new();

		public ShopService(RabbitMQService rabbitMQService, IBaseRepository<Shop> shopRepository)
		{
			_rabbit = rabbitMQService;
			_shopRepository = shopRepository;
		}

		public async Task<Shop?> CreateShopAsync(string title, CancellationToken cancellationToken = default)
		{
			var shop = new Shop
			{
				Title = title
			};

			_shopRepository.Create(shop);
			await _shopRepository.SaveChangesAsync(cancellationToken);

			var consumer = await _rabbit.AddConsumer<MigrateDbResponse>("MigrateResponse", response =>
			{
				_migrateResponses.TryAdd(response.ShopId, response);
				return Task.CompletedTask;
			});

			var migrateRequest = new MigrateDbRequest
			{
				ShopId = shop.Id,
			};

			await _rabbit.PublishMessage(migrateRequest, "ShopExchange", "MigrateRequest", cancellationToken);

			MigrateDbResponse? response = default;

			while (true)
			{
				if (_migrateResponses.TryGetValue(shop.Id, out response))
				{
					break;
				}
			}

			if (response != null)
			{
				if (response.IsSuccess)
				{
					return shop;
				}
			}

			return null;
		}
	}
}
