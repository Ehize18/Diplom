using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopService.Application.Services;
using ShopService.Contracts.Responses;

namespace ShopService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly CategoryService _categoryService;

		public CategoryController(CategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		[HttpGet("byParent")]
		public async Task<ActionResult> GetCategoriesByParent(Guid? parentCategoryId, CancellationToken cancellationToken = default)
		{
			var categories = await _categoryService.GetCategoriesByParent(parentCategoryId, cancellationToken);

			if (categories != null)
			{
				return Ok(categories);
			}
			return BadRequest();
		}

		[HttpGet("{categoryId:guid}")]
		public async Task<ActionResult> GetCategoryById(Guid categoryId, bool withChilds = true, CancellationToken cancellationToken = default)
		{
			var category = await _categoryService.GetCategoryById(categoryId, withChilds, cancellationToken);

			if (category != null)
			{
				return Ok(new GoodCategoryResponse(category));
			}
			return NotFound();
		}
	}
}
