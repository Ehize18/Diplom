using AdministrativeService.Application.DTO.Shop;
using AdministrativeService.Application.Services;
using AdministrativeService.Contracts.Shop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministrativeService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShopController : ControllerWithUser
	{
		private readonly ShopService _shopService;

		public ShopController(ShopService shopService)
		{
			_shopService = shopService;
		}

		[HttpPost]
		public async Task<ActionResult<ShopResponse>> CreateShop(CreateShopRequest request, CancellationToken cancellationToken)
		{
			var dto = new CreateShopDTO
			{
				Title = request.Title,
				User = CurrentUser
			};
			var shop = await _shopService.CreateShopAsync(dto, cancellationToken);

			if (shop == null)
			{
				return BadRequest();
			}
			return Ok(new ShopResponse(shop));
		}

		[HttpGet]
		public async Task<ActionResult<List<ShopResponse>>> GetUserShops()
		{
			var shops = await _shopService.GetUserShopsAsync(CurrentUser.Id);

			var response = new List<ShopResponse>();

			foreach (var shop in shops)
			{
				response.Add(new(shop));
			}

			return Ok(response);
		}
	}
}
