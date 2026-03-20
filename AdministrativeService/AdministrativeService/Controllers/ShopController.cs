using AdministrativeService.Application.DTO.Shop;
using AdministrativeService.Application.Services;
using AdministrativeService.Contracts.Shop;
using AdministrativeService.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministrativeService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShopController : ControllerBase
	{
		private readonly ShopService _shopService;

		private User CurrentUser
		{
			get
			{
				return new User
				{
					Id = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value),
					Username = User.Claims.First(c => c.Type == "Username").Value
				};
			}
		}

		public ShopController(ShopService shopService)
		{
			_shopService = shopService;
		}

		[HttpPost]
		[Authorize]
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
		[Authorize]
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
