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

		private (Guid, string) GetResponse(PropertyUpdated? propertyUpdated)
		{
			if (propertyUpdated == null)
			{
				return (Guid.Empty, "unexpected_error");
			}
			if (propertyUpdated.IsSuccess)
			{
				return (propertyUpdated.PropertyId, string.Empty);
			}
			return (
				propertyUpdated.PropertyId != Guid.Empty ? propertyUpdated.PropertyId : Guid.Empty,
				propertyUpdated.Error != null ? propertyUpdated.Error : "unexpected_error");
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

		private (Guid, string) GetResponse(PropertyValueUpdated? propertyValueUpdated)
		{
			if (propertyValueUpdated == null)
			{
				return (Guid.Empty, "unexpected_error");
			}
			if (propertyValueUpdated.IsSuccess)
			{
				return (propertyValueUpdated.PropertyValueId, string.Empty);
			}
			return (
				propertyValueUpdated.PropertyValueId != Guid.Empty ? propertyValueUpdated.PropertyValueId : Guid.Empty,
				propertyValueUpdated.Error != null ? propertyValueUpdated.Error : "unexpected_error");
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
				ShopId = dto.ShopId,
				ImageId = dto.ImageId
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
				ShopId = dto.ShopId,
				ImageId = dto.ImageId
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
				ImageId = dto.ImageId,
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

		public async Task<(Guid, string)> CreateProperty(CreatePropertyDTO dto, CancellationToken cancellationToken = default)
		{
			var updateProperty = new UpdateProperty
			{
				PropertyTitle = dto.Title,
				UpdateType = UpdateType.Create,
				ShopId = dto.ShopId,
				UpdatedById = dto.User.Id
			};

			var response = await _messageService.UpdateProperty(updateProperty, cancellationToken);

			return GetResponse(response);
		}

		public async Task<(Guid, string)> UpdateProperty(PatchPropertyDTO dto, CancellationToken cancellationToken = default)
		{
			var updateProperty = new UpdateProperty
			{
				PropertyId = dto.PropertyId,
				PropertyTitle = dto.Title,
				UpdateType = UpdateType.Update,
				ShopId = dto.ShopId,
				UpdatedById = dto.User.Id
			};

			var response = await _messageService.UpdateProperty(updateProperty, cancellationToken);

			return GetResponse(response);
		}

		public async Task<(Guid, string)> CreatePropertyValue(CreatePropertyValueDTO dto, CancellationToken cancellationToken = default)
		{
			var updateProperty = new UpdatePropertyValue
			{
				PropertyTitle = dto.Title,
				UpdateType = UpdateType.Create,
				PropertyId = dto.PropertyId,
				ShopId = dto.ShopId,
				UpdatedById = dto.User.Id
			};

			var response = await _messageService.UpdatePropertyValue(updateProperty, cancellationToken);

			return GetResponse(response);
		}

		public async Task<(Guid, string)> UpdatePropertyValue(PatchPropertyValueDTO dto, CancellationToken cancellationToken = default)
		{
			var updateProperty = new UpdatePropertyValue
			{
				PropertyValueId = dto.PropertyValueId,
				PropertyTitle = dto.Title,
				UpdateType = UpdateType.Update,
				ShopId = dto.ShopId,
				UpdatedById = dto.User.Id
			};

			var response = await _messageService.UpdatePropertyValue(updateProperty, cancellationToken);

			return GetResponse(response);
		}

		public async Task<(Guid, string)> PatchGood(PatchGoodDTO dto, CancellationToken cancellationToken = default)
		{
			var updateGood = new UpdateGood
			{
				GoodId = dto.GoodId,
				UpdateType = UpdateType.Update,
				ShopId = dto.ShopId,
				UpdatedById = dto.User.Id,
				GoodTitle = dto.Title,
				GoodDescription = dto.Description,
				CategoryId = dto.CategoryId ?? Guid.Empty,
				Count = dto.Count ?? 0,
				Price = dto.Price ?? 0,
				OldPrice = dto.OldPrice ?? 0,
				ImageId = dto.ImageId
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
	}
}
