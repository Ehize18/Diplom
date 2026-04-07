using Microsoft.AspNetCore.Mvc;
using ShopService.Application.Services;
using ShopService.Contracts.Responses;
using ShopService.Core.Entities;

namespace ShopService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerWithUser
	{
		private readonly OrderService _orderService;

		public OrderController(OrderService orderService)
		{
			_orderService = orderService;
		}

		[HttpPost]
		public async Task<ActionResult> CreateOrder(CreateOrderRequest request, CancellationToken cancellationToken = default)
		{
			await _orderService.CreateOrder(CurrentUser, request.PaymentMethodId, request.DeliveryMethodId, request.DeliveryExtras, cancellationToken);
			return Ok();
		}

		[HttpGet]
		public async Task<ActionResult> GetOrders(CancellationToken cancellationToken = default)
		{
			var orders = await _orderService.GetOrders(CurrentUser, cancellationToken);
			return Ok(orders);
		}

		[HttpGet("{orderId:guid}")]
		public async Task<ActionResult> GetOrderById(Guid orderId, bool withBasket = false, CancellationToken cancellationToken = default)
		{
			var order = await _orderService.GetOrderById(CurrentUser, orderId, withBasket, cancellationToken);
			if (order == null)
			{
				return NotFound(orderId);
			}
			return Ok(new OrderResponse(order));
		}

		public class OrderResponse : Order
		{
			public new BasketResponse? Basket { get; set; }

			public OrderResponse(Order order)
			{
				this.Id = order.Id;
				this.BasketId = order.BasketId;
				this.DeliveryMethodId = order.DeliveryMethodId;
				this.PaymentMethodId = order.PaymentMethodId;
				this.CreatedAt = order.CreatedAt;
				this.DeliveryStatus = order.DeliveryStatus;
				this.PaymentStatus = order.PaymentStatus;
				this.DeliveryExtras = order.DeliveryExtras;
				if (order.Basket != null)
				{
					this.Basket = new BasketResponse(order.Basket);
				}
				
			}
		}

		public record CreateOrderRequest(
			Guid DeliveryMethodId,
			Guid PaymentMethodId,
			string DeliveryExtras
			);
	}
}
