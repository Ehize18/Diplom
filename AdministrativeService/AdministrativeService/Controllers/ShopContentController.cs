using AdministrativeService.Application.DTO.ShopContent;
using AdministrativeService.Application.Services;
using AdministrativeService.Contracts.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.RabbitMQ.Contracts;

namespace AdministrativeService.Controllers
{
	[Route("api/[controller]/{shopId:guid}")]
	[ApiController]
	[Authorize]
	public class ShopContentController : ControllerWithUser
	{
		private readonly ShopContentService _shopContentService;

		public ShopContentController(ShopContentService shopContentService)
		{
			_shopContentService = shopContentService;
		}

		[HttpPost("category")]
		public async Task<ActionResult> CreateCategory(CreateCategoryRequest request, Guid shopId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.CreateCategory(new()
			{
				ShopId = shopId,
				Title = request.Title,
				Description = request.Description,
				ParentCategoryId = request.ParentCategoryId,
				IsActive = request.IsActive,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("category/{categoryId:guid}")]
		public async Task<ActionResult> PutCategory(UpdateCategoryRequest request, Guid shopId, Guid categoryId, CancellationToken cancellationToken = default)
		{
			Guid? parentCategoryId = null;
			if (Guid.TryParse(request.ParentCategoryId, out var parentCategoryIdParsed))
			{
				parentCategoryId = parentCategoryIdParsed;
			}
			var (id, error) = await _shopContentService.PatchCategory(new()
			{
				ShopId = shopId,
				CategoryId = categoryId,
				Title = request.Title,
				Description = request.Description,
				IsActive = request.IsActive,
				ParentCategoryId = parentCategoryId,
				User = CurrentUser
			});

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPost("good")]
		public async Task<ActionResult> CreateGood(CreateGoodRequest request, Guid shopId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.CreateGood(new()
			{
				ShopId = shopId,
				Title = request.Title,
				Description = request.Description,
				CategoryId = request.CategoryId,
				Count = request.Count,
				Price = request.Price,
				OldPrice = request.OldPrice,
				User = CurrentUser
			});

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
	}
}
