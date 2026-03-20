using System.Collections.Concurrent;
using AdministrativeService.Application.DTO.Shop;
using AdministrativeService.Core.Entities;
using AdministrativeService.Core.Enums;
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

		public async Task<Shop?> CreateShopAsync(CreateShopDTO createShopDTO, CancellationToken cancellationToken = default)
		{
			var shop = new Shop
			{
				Title = createShopDTO.Title,
				Admins = new List<ShopAdmin>
				{
					new ShopAdmin
					{
						UserId = createShopDTO.User.Id,
						Feature = AdminFeature.CanAll
					}
				}
			};
			_shopRepository.SetUser(createShopDTO.User);
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

			var tasksToWait = new List<Task>();

			var getShopTask = GetShopById(shop.Id);

			tasksToWait.Add(getShopTask);

			tasksToWait.Add(_rabbit.PublishMessage(migrateRequest, "ShopExchange", "MigrateRequest", cancellationToken));

			var migrateTask = GetMigrateDbResponse(shop.Id);

			tasksToWait.Add(migrateTask);

			await Task.WhenAll(tasksToWait);

			var response = migrateTask.Result;

			await _rabbit.RemoveConsumer(consumer);

			if (response != null)
			{
				if (response.IsSuccess)
				{
					return getShopTask.Result;
				}
			}

			return null;
		}

		private async Task<MigrateDbResponse> GetMigrateDbResponse(Guid shopId)
		{
			return await Task.Run(() =>
			{
				while (true)
				{
					if (_migrateResponses.TryRemove(shopId, out var response))
					{
						return response;
					}
				}
			});
		}

		public async Task<List<Shop>> GetUserShopsAsync(Guid userId)
		{
			var shops = await _shopRepository.GetAsync(shop => shop.Admins.Any(x => x.UserId == userId));

			return shops;
		}

		public async Task<Shop?> GetShopById(Guid shopId)
		{
			return await _shopRepository.GetByIdAsync(shopId);
		}
	}
}
