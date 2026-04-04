using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopService.Application.Services;
using ShopService.Contracts.Requests;
using ShopService.Contracts.Responses;

namespace ShopService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BasketController : ControllerWithUser
	{
		private readonly BasketService _basketService;

		public BasketController(BasketService basketService)
		{
			_basketService = basketService;
		}

		[HttpPost("Good")]
		public async Task<ActionResult> AddGoodToBasket(AddGoodToBasketRequest request, CancellationToken cancellationToken = default)
		{
			var (isSuccess, error) = await _basketService.AddGoodToCurrentBasket(CurrentUser, request.GoodId, request.Count, cancellationToken);
			if (isSuccess)
			{
				return Ok();
			}
			return BadRequest(error);
		}

		[HttpGet]
		public async Task<ActionResult<BasketResponse>> GetCurrentBasket(CancellationToken cancellationToken = default)
		{
			var basket = await _basketService.GetCurrentBasketWithItems(CurrentUser, cancellationToken);

			if (basket == null)
			{
				return BadRequest();
			}

			return Ok(new BasketResponse(basket));
		}

		[HttpPatch("item/{basketItemId:guid}")]
		public async Task<ActionResult<int>> ChangeBasketItem(Guid basketItemId, ChangeBasketItemCountRequest request, CancellationToken cancellationToken = default)
		{
			var result = await _basketService
				.ChangeBasketItemCount(CurrentUser, basketItemId, request.Count, cancellationToken);

			return Ok(result);
		}

		[HttpDelete("item/{basketItemId:guid}")]
		public async Task<ActionResult<int>> DeleteItemFromBasket(Guid basketItemId, CancellationToken cancellationToken = default)
		{
			var result = await _basketService
				.DeleteBasketItem(CurrentUser, basketItemId, cancellationToken);

			return Ok(result);
		}
	}
}
