using Microsoft.AspNetCore.Mvc;
using ShopService.Application.Services;
using ShopService.Core.Entities;

namespace ShopService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GoodController : ControllerBase
	{
		private readonly GoodService _goodService;

		public GoodController(GoodService goodService)
		{
			_goodService = goodService;
		}

		[HttpGet]
		public async Task<ActionResult> GetGoods(Guid? categoryId, bool? isActual, CancellationToken cancellationToken = default)
		{
			IEnumerable<Good?> result;
			if (categoryId == null || isActual == null)
			{
				result = await _goodService.GetGoods(1, 20, cancellationToken);
			}
			else
			{
				result = await _goodService.GetGoodsByCategory((Guid)categoryId, (bool)isActual, cancellationToken);
			}
			
			return Ok(result);
		}

		[HttpGet("{goodId:guid}")]
		public async Task<ActionResult> GetGoodById(Guid goodId, CancellationToken cancellationToken = default)
		{
			var result = await _goodService.GetGoodById(goodId, cancellationToken);
			if (result == null)
			{
				return NotFound();
			}
			return Ok(result);
		}
	}
}
