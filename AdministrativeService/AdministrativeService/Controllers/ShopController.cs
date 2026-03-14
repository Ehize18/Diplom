using AdministrativeService.Application.Services;
using AdministrativeService.Core.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdministrativeService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShopController : ControllerBase
	{
		private readonly ShopService _shopService;

		public ShopController(ShopService shopService)
		{
			_shopService = shopService;
		}

		[HttpPost]
		public async Task<IActionResult> CreateShop(CreateShopRequest request, CancellationToken cancellationToken)
		{
			var shop = await _shopService.CreateShopAsync(request.Title, cancellationToken);

			if (shop == null)
			{
				return BadRequest();
			}
			return Ok(shop);
		}
	}
}
