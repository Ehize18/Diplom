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
				ParentCategoryId = request.ParentCategory,
				IsActive = request.IsActive,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpGet("category")]
		public async Task<ActionResult> GetCategories(
			Guid shopId, string? orderBy, bool? isAscending, int? page, int? pageSize,
			FilterType? filterType, string? column, string? columnValue,
			CancellationToken cancellationToken = default)
		{
			var dto = new GetCategoriesDTO
			{
				Entity = DataGetEntity.Category,
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

			if (filterType != null && !string.IsNullOrWhiteSpace(column) && columnValue != default)
			{
				dto.Filter = new Filter
				{
					FilterType = (FilterType)filterType,
					LeftExpression = column,
					RightExpression = columnValue
				};
			}

			var response = await _shopContentService.GetCategories(dto, cancellationToken);

			if (response == null)
			{
				return BadRequest();
			}
			return Ok(response);
		}
	}
}
