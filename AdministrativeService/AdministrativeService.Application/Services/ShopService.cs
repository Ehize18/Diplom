using AdministrativeService.Application.DTO.Shop;
using AdministrativeService.Core.Entities;
using AdministrativeService.Core.Enums;
using AdministrativeService.Database.Interfaces;
using Shared.Enums;
using Shared.RabbitMQ.Contracts;

namespace AdministrativeService.Application.Services
{
	public class ShopService
	{
		private readonly MessageService _messageService;
		private readonly IBaseRepository<Shop> _shopRepository;

		public ShopService(MessageService messageService, IBaseRepository<Shop> shopRepository)
		{
			_messageService = messageService;
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

			var createShopRequest = new CreateShop
			{
				ShopId = shop.Id,
			};

			var tasksToWait = new List<Task>();

			var getShopTask = GetShopById(shop.Id);

			tasksToWait.Add(getShopTask);

			var messageId = Guid.NewGuid();

			var properties = _messageService.CreateProperties();

			properties.CorrelationId = messageId.ToString();
			properties.Persistent = true;

			var answerTask = _messageService.GetAnswerAsync<ShopCreated>(messageId, cancellationToken);

			tasksToWait.Add(answerTask);

			tasksToWait.Add(_messageService.PublishCreateShopMessage(properties, createShopRequest, cancellationToken));
			
			await Task.WhenAll(tasksToWait);

			var response = answerTask.Result;

			if (response != null)
			{
				if (response.IsSuccess)
				{
					return getShopTask.Result;
				}
			}

			return null;
		}

		public async Task<List<Shop>> GetUserShopsAsync(Guid userId)
		{
			var shops = await _shopRepository.GetAsync(shop => shop.Admins.Any(x => x.UserId == userId));

			return shops;
		}

		public async Task<Shop?> UpdateShop(User user, Guid shopId, long? vkGroupId, CancellationToken cancellationToken = default)
		{
			var shops = await _shopRepository.GetAsync(x => x.Id == shopId && x.Admins.Any(x => x.UserId == user.Id && x.Feature == AdminFeature.CanAll));
			var shop = shops.FirstOrDefault();
			if (shop != null)
			{
				shop.VkGroupId = vkGroupId;
				shop = _shopRepository.Update(shop);
				await _shopRepository.SaveChangesAsync(cancellationToken);
			}
			return shop;
		}

		public async Task<Shop?> GetShopById(Guid shopId)
		{
			return await _shopRepository.GetByIdAsync(shopId);
		}

		public async Task<MethodUpdated?> CreateDeliveryMethod(User user, Guid shopId, string title, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
		{
			var update = new UpdateMethod
			{
				MethodType = MethodType.Delivery,
				UpdateType = UpdateType.Create,
				Title = title,
				MetadataBody = metadata,
				ShopId = shopId,
				UpdatedById = user.Id
			};

			var result = await _messageService.UpdateMethod(update, cancellationToken);

			return result;
		}

		public async Task<MethodUpdated?> CreatePaymentMethod(User user, Guid shopId, string title, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
		{
			var update = new UpdateMethod
			{
				MethodType = MethodType.Payment,
				UpdateType = UpdateType.Create,
				Title = title,
				MetadataBody = metadata,
				ShopId = shopId,
				UpdatedById = user.Id
			};

			var result = await _messageService.UpdateMethod(update, cancellationToken);

			return result;
		}

		public async Task<MethodUpdated?> UpdateDeliveryMethod(User user, Guid shopId, Guid methodId, string title, CancellationToken cancellationToken = default)
		{
			var update = new UpdateMethod
			{
				MethodType = MethodType.Delivery,
				UpdateType = UpdateType.Update,
				MethodId = methodId,
				Title = title,
				ShopId = shopId,
				UpdatedById = user.Id
			};

			var result = await _messageService.UpdateMethod(update, cancellationToken);

			return result;
		}

		public async Task<MethodUpdated?> UpdatePaymentMethod(User user, Guid shopId, Guid methodId, string title, CancellationToken cancellationToken = default)
		{
			var update = new UpdateMethod
			{
				MethodType = MethodType.Payment,
				UpdateType = UpdateType.Update,
				MethodId = methodId,
				Title = title,
				ShopId = shopId,
				UpdatedById = user.Id
			};

			var result = await _messageService.UpdateMethod(update, cancellationToken);

			return result;
		}
	}
}
