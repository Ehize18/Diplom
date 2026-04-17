using AdministrativeService.Application.DTO.ShopContent;
using AdministrativeService.Application.Services;
using AdministrativeService.Contracts.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.RabbitMQ.Contracts;
using Shared.S3;

namespace AdministrativeService.Controllers
{
	[Route("api/[controller]/{shopId:guid}")]
	[ApiController]
	[Authorize]
	public class ShopContentController : ControllerWithUser
	{
		private readonly ShopContentService _shopContentService;
		private readonly IMinioService _minioService;

		public ShopContentController(ShopContentService shopContentService, IMinioService minioService)
		{
			_shopContentService = shopContentService;
			_minioService = minioService;
		}

		[HttpPost("category")]
		public async Task<ActionResult> CreateCategory([FromForm]CreateCategoryRequest request, IFormFile? image, Guid shopId, CancellationToken cancellationToken = default)
		{
			Guid? imageId = null;
			var tasksToWait = new List<Task>();
			if (image != null)
			{
				imageId = Guid.NewGuid();
				tasksToWait.Add(UploadImage((Guid)imageId, shopId, image, cancellationToken));
			}
			var createCategoryTask = _shopContentService.CreateCategory(new()
			{
				ShopId = shopId,
				Title = request.Title,
				Description = request.Description,
				ParentCategoryId = request.ParentCategoryId,
				IsActive = request.IsActive,
				User = CurrentUser,
				ImageId = imageId,
			}, cancellationToken);
			tasksToWait.Add(createCategoryTask);

			await Task.WhenAll(tasksToWait);

			var (id, error) = createCategoryTask.Result;

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("category/{categoryId:guid}")]
		public async Task<ActionResult> PutCategory([FromForm]UpdateCategoryRequest request, IFormFile? image, Guid shopId, Guid categoryId, CancellationToken cancellationToken = default)
		{
			Guid? imageId = request.ImageId;
			var tasksToWait = new List<Task>();
			if (image != null)
			{
				imageId = Guid.NewGuid();
				tasksToWait.Add(UploadImage((Guid)imageId, shopId, image, cancellationToken));
			}
			Guid? parentCategoryId = null;
			if (Guid.TryParse(request.ParentCategoryId, out var parentCategoryIdParsed))
			{
				if (parentCategoryIdParsed != Guid.Empty)
				{
					parentCategoryId = parentCategoryIdParsed;
				}
			}
			var patchTask = _shopContentService.PatchCategory(new()
			{
				ShopId = shopId,
				CategoryId = categoryId,
				Title = request.Title,
				Description = request.Description,
				IsActive = request.IsActive,
				ParentCategoryId = parentCategoryId,
				ImageId = imageId,
				User = CurrentUser
			});
			tasksToWait.Add(patchTask);
			await Task.WhenAll(tasksToWait);
			var (id, error) = patchTask.Result;

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpDelete("category/{categoryId:guid}")]
		public async Task<ActionResult> DeleteCategory(Guid shopId, Guid categoryId, string? imageId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.DeleteCategory(new()
			{
				ShopId = shopId,
				CategoryId = categoryId,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				if (!string.IsNullOrWhiteSpace(imageId) && Guid.TryParse(imageId, out var imageGuid))
				{
					await _minioService.DeleteObject(shopId.ToString(), imageId, cancellationToken);
				}
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPost("good")]
		public async Task<ActionResult> CreateGood([FromForm]CreateGoodRequest request, IFormFile? image, Guid shopId, CancellationToken cancellationToken = default)
		{
			Guid? imageId = null;
			var tasksToWait = new List<Task>();
			if (image != null)
			{
				imageId = Guid.NewGuid();
				tasksToWait.Add(UploadImage((Guid)imageId, shopId, image, cancellationToken));
			}

			var createGoodTask = _shopContentService.CreateGood(new()
			{
				ShopId = shopId,
				Title = request.Title,
				Description = request.Description,
				CategoryId = request.CategoryId,
				Count = request.Count,
				Price = request.Price,
				OldPrice = request.OldPrice,
				ImageId = imageId,
				User = CurrentUser
			});

			await Task.WhenAll(tasksToWait);

			var (id, error) = createGoodTask.Result;

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("good/{goodId:guid}")]
		public async Task<ActionResult> PutGood([FromForm]UpdateGoodRequest request, IFormFile? image, Guid shopId, Guid goodId, CancellationToken cancellationToken = default)
		{
			Guid? imageId = request.ImageId;
			var tasksToWait = new List<Task>();
			if (image != null)
			{
				imageId = Guid.NewGuid();
				tasksToWait.Add(UploadImage((Guid)imageId, shopId, image, cancellationToken));
			}
			Guid? categoryId = null;
			if (Guid.TryParse(request.CategoryId, out var categoryIdParsed))
			{
				if (categoryIdParsed != Guid.Empty)
				{
					categoryId = categoryIdParsed;
				}
			}
			var patchTask = _shopContentService.PatchGood(new()
			{
				ShopId = shopId,
				GoodId = goodId,
				Title = request.Title,
				Description = request.Description,
				CategoryId = categoryId,
				Count = request.Count,
				Price = request.Price,
				OldPrice = request.OldPrice,
				ImageId = imageId,
				User = CurrentUser
			});
			tasksToWait.Add(patchTask);
			await Task.WhenAll(tasksToWait);
			var (id, error) = patchTask.Result;

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpDelete("good/{goodId:guid}")]
		public async Task<ActionResult> DeleteGood(Guid shopId, Guid goodId, string? imageId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.DeleteGood(new()
			{
				ShopId = shopId,
				GoodId = goodId,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				if (!string.IsNullOrWhiteSpace(imageId) && Guid.TryParse(imageId, out var imageGuid))
				{
					await _minioService.DeleteObject(shopId.ToString(), imageId, cancellationToken);
				}
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("order/{orderId:guid}")]
		public async Task<ActionResult> PutOrder([FromBody]UpdateOrderRequest request, Guid shopId, Guid orderId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.PatchOrder(new()
			{
				ShopId = shopId,
				OrderId = orderId,
				EntityType = request.EntityType,
				StatusValue = request.StatusValue,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpGet("{entityType}")]
		public async Task<ActionResult> GetData(string entityType,
			Guid shopId, string? orderBy, bool? isAscending, int? page, int? pageSize,
			FilterType? filterType, string? column, string? columnValue,
			CancellationToken cancellationToken = default)
		{
			if (Enum.TryParse<DataGetEntity>(entityType, true, out var type))
			{
				var dto = new GetDataDTO
				{
					Entity = type,
					ShopId = shopId
				};

				if (orderBy != null)
				{
					dto.OrderBy = orderBy;
					dto.IsAscending = isAscending != null ? (bool)isAscending : false;
				}

				if (page != null && page > 0 && pageSize != null && pageSize > 0)
				{
					dto.Page = (int)page;
					dto.PageSize = (int)pageSize;
				}

				if (filterType != null && !string.IsNullOrWhiteSpace(column))
				{
					dto.Filter = new Filter
					{
						FilterType = (FilterType)filterType,
						LeftExpression = column,
						RightExpression = columnValue
					};
				}

				var response = await _shopContentService.GetData(dto, cancellationToken);

				if (response == null)
				{
					return BadRequest();
				}
				return Ok(response);
			}
			return BadRequest("unknown_type");
		}

		[HttpPost("property")]
		public async Task<ActionResult> CreateProperty(CreatePropertyRequest request, Guid shopId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.CreateProperty(new()
			{
				ShopId = shopId,
				Title = request.Title,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("property/{propertyId:guid}")]
		public async Task<ActionResult> UpdateProperty(CreatePropertyRequest request, Guid shopId, Guid propertyId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.UpdateProperty(new()
			{
				PropertyId = propertyId,
				ShopId = shopId,
				Title = request.Title,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPost("property/{propertyId:guid}")]
		public async Task<ActionResult> CreatePropertyValue(CreatePropertyRequest request, Guid shopId, Guid propertyId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.CreatePropertyValue(new()
			{
				PropertyId = propertyId,
				ShopId = shopId,
				Title = request.Title,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("property/{propertyId:guid}/{propertyValueId:guid}")]
		public async Task<ActionResult> UpdateProperty(CreatePropertyRequest request, Guid shopId, Guid propertyId, Guid propertyValueId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.UpdatePropertyValue(new()
			{
				PropertyValueId = propertyValueId,
				ShopId = shopId,
				Title = request.Title,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPost("image")]
		public async Task<ActionResult> UploadImage(Guid shopId, IFormFile image, CancellationToken cancellationToken = default)
		{
			var imageId = Guid.NewGuid();

			await UploadImage(imageId, shopId, image, cancellationToken);

			return Ok(imageId);
		}

		private async Task UploadImage(Guid imageId, Guid shopId, IFormFile image, CancellationToken cancellationToken = default)
		{
			var policyJson = @"{
				""Version"": ""2012-10-17"",
				""Statement"": [{
					""Effect"": ""Allow"",
					""Principal"": { ""AWS"": ""*"" },
					""Action"": [""s3:GetObject""],
					""Resource"": [""arn:aws:s3:::" + shopId.ToString() + @"/*""]
				}]
			}";
			await _minioService.Init(shopId.ToString(), policyJson, cancellationToken);

			using (var stream = new MemoryStream())
			{
				await image.CopyToAsync(stream);
				stream.Position = 0;
				await _minioService.PutObject(shopId.ToString(), imageId.ToString(), stream, image.ContentType, cancellationToken);
			}
		}

		[HttpDelete("image/{imageId:guid}")]
		public async Task<ActionResult> DeleteImage(Guid shopId, Guid imageId, CancellationToken cancellationToken = default)
		{
			await _minioService.DeleteObject(shopId.ToString(), imageId.ToString(), cancellationToken);
			return Ok();
		}
	}
}
