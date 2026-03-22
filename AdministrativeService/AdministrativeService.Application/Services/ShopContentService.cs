using AdministrativeService.Application.DTO.ShopContent;
using Shared.RabbitMQ.Contracts;

namespace AdministrativeService.Application.Services
{
	public class ShopContentService
	{
		private readonly MessageService _messageService;

		public ShopContentService(MessageService messageService)
		{
			_messageService = messageService;
		}

		private (Guid, string) GetResponse(CategoryUpdated? categoryUpdated)
		{
			if (categoryUpdated == null)
			{
				return (Guid.Empty, "unexpected_error");
			}
			if (categoryUpdated.IsSuccess)
			{
				return (categoryUpdated.CategoryId, string.Empty);
			}
			return (
				categoryUpdated.CategoryId != Guid.Empty ? categoryUpdated.CategoryId : Guid.Empty,
				categoryUpdated.Error != null ? categoryUpdated.Error : "unexpected_error");
		}

		private (Guid, string) GetResponse(GoodUpdated? goodUpdated)
		{
			if (goodUpdated == null)
			{
				return (Guid.Empty, "unexpected_error");
			}
			if (goodUpdated.IsSuccess)
			{
				return (goodUpdated.GoodId, string.Empty);
			}
			return (
				goodUpdated.GoodId != Guid.Empty ? goodUpdated.GoodId : Guid.Empty,
				goodUpdated.Error != null ? goodUpdated.Error : "unexpected_error");
		}

		public async Task<(Guid, string)> CreateCategory(CreateCategoryDTO dto, CancellationToken cancellationToken = default)
		{
			var updateCategory = new UpdateCategory
			{
				UpdateType = UpdateType.Create,
				CategoryTitle = dto.Title,
				CategoryDescription = dto.Description,
				ParentCategoryId = dto.ParentCategoryId,
				IsActive = dto.IsActive,
				UpdatedById = dto.User.Id,
				ShopId = dto.ShopId
			};

			var messageId = Guid.NewGuid();

			var responseTask = _messageService.GetAnswerAsync<CategoryUpdated>(messageId, cancellationToken);

			var properties = _messageService.CreateProperties();
			properties.CorrelationId = messageId.ToString();

			var requestTask = _messageService.PublishUpdateCategoryMessage(properties, updateCategory, cancellationToken);

			await Task.WhenAll(responseTask, requestTask);

			var response = responseTask.Result;

			return GetResponse(response);
		}

		public async Task<(Guid, string)> CreateGood(CreateGoodDTO dto, CancellationToken cancellationToken = default)
		{
			var updateGood = new UpdateGood
			{
				UpdateType = UpdateType.Create,
				GoodTitle = dto.Title,
				GoodDescription = dto.Description,
				CategoryId = dto.CategoryId,
				Count = dto.Count,
				Price = dto.Price,
				OldPrice = dto.OldPrice,
				UpdatedById = dto.User.Id,
				ShopId = dto.ShopId
			};

			var messageId = Guid.NewGuid();

			var responseTask = _messageService.GetAnswerAsync<GoodUpdated>(messageId, cancellationToken);

			var properties = _messageService.CreateProperties();
			properties.CorrelationId = messageId.ToString();

			var requestTask = _messageService.PublishUpdateGoodMessage(properties, updateGood, cancellationToken);

			await Task.WhenAll(responseTask, requestTask);

			var response = responseTask.Result;

			return GetResponse(response);
		}

		public async Task<DataGetResponse?> GetData(
			GetDataDTO dto,  CancellationToken cancellationToken = default)
		{
			var dataGet = new DataGet
			{
				ShopId = dto.ShopId,
				Entity = dto.Entity,
				OrderBy = dto.OrderBy,
				IsAscending = dto.IsAscending,
				Page = dto.Page,
				PageSize = dto.PageSize,
				Filter = dto.Filter
			};

			var messageId = Guid.NewGuid();

			var responseTask = _messageService.GetAnswerAsync<DataGetResponse>(messageId, cancellationToken);

			var properties = _messageService.CreateProperties();
			properties.CorrelationId = messageId.ToString();

			var requestTask = _messageService.PublishDataGetMessage(properties, dataGet, cancellationToken);

			await Task.WhenAll(responseTask, requestTask);

			return responseTask.Result;
		}



		public async Task<(Guid, string)> PatchCategory(PatchCategoryDTO dto, CancellationToken cancellationToken = default)
		{
			var updateCategory = new UpdateCategory
			{
				ShopId = dto.ShopId,
				UpdateType = UpdateType.Update,
				UpdatedById = dto.User.Id,
				CategoryId = dto.CategoryId,
				CategoryTitle = dto.Title,
				CategoryDescription = dto.Description,
				IsActive = dto.IsActive,
				ParentCategoryId = dto.ParentCategoryId
			};

			var messageId = Guid.NewGuid();

			var responseTask = _messageService.GetAnswerAsync<CategoryUpdated>(messageId, cancellationToken);

			var properties = _messageService.CreateProperties();
			properties.CorrelationId = messageId.ToString();

			var requestTask = _messageService.PublishUpdateCategoryMessage(properties, updateCategory, cancellationToken);

			await Task.WhenAll(responseTask, requestTask);

			var response = responseTask.Result;

			return GetResponse(response);
		}
	}
}
