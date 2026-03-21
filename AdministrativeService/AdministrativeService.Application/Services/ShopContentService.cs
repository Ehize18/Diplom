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

		public async Task<DataGetResponse?> GetCategories(
			GetCategoriesDTO dto,  CancellationToken cancellationToken = default)
		{
			var dataGet = new DataGet
			{
				ShopId = dto.ShopId,
				Entity = DataGetEntity.Category,
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
	}
}
