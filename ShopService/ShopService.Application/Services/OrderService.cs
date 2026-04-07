using Shared.Enums;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Application.Services
{
	public class OrderService
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IBaseRepository<Order> _orderRepository;

		public OrderService(IBasketRepository basketRepository, IBaseRepository<Order> orderRepository)
		{
			_basketRepository = basketRepository;
			_orderRepository = orderRepository;
		}

		public async Task CreateOrder(
			User user,
			Guid paymentMethodId,
			Guid deliveryMethodId,
			string deliveryExtras,
			CancellationToken cancellationToken = default)
		{
			_basketRepository.SetUser(user);
			var basket = await _basketRepository.GetCurrentBasket(true, cancellationToken);

			if (basket == null)
			{
				return;
			}

			if (!CheckItemsCount(basket))
			{
				return;
			}

			UpdateItemsCount(basket);

			var order = new Order
			{
				FullPrice = CalculatePrice(basket),
				BasketId = basket.Id,
				PaymentMethodId = paymentMethodId,
				DeliveryMethodId = deliveryMethodId,
				DeliveryExtras = deliveryExtras,
				PaymentStatus = PaymentStatus.Created,
				DeliveryStatus = DeliveryStatus.Created,
				OrderStatus = OrderStatus.Created
			};

			basket.IsCurrent = false;
			_basketRepository.Update(basket);
			_orderRepository.Create(order);

			await _orderRepository.SaveChangesAsync();
		}

		public async Task<List<Order>> GetOrders(User user, CancellationToken cancellationToken = default)
		{
			var orders = await _orderRepository.GetAsync(x => x.Basket.UserId == user.Id, "CreatedAt");
			return orders;
		}

		public async Task<Order?> GetOrderById(User user, Guid orderId, bool withBasket, CancellationToken cancellationToken = default)
		{
			var order = (await _orderRepository.GetAsync(x => x.Basket.UserId == user.Id && x.Id == orderId, "CreatedAt")).FirstOrDefault();

			if (withBasket && order != null)
			{
				var basket = await _basketRepository.GetByIdAsync(order.BasketId);
				//basket = await _basketRepository
				order.Basket = basket;
			}

			return order;
		}

		private bool CheckItemsCount(Basket basket)
		{
			foreach (var item in basket.Goods)
			{
				if (item.Count > item.Good.Count)
				{
					return false;
				}
			}
			return true;
		}

		private void UpdateItemsCount(Basket basket)
		{
			foreach (var item in basket.Goods)
			{
				item.Good.Count -= item.Count;
			}
		}

		private decimal CalculatePrice(Basket basket)
		{
			decimal result = 0;
			foreach (var item in basket.Goods)
			{
				item.Price = item.Good.Price;
				result += item.Price * item.Count;
			}
			return result;
		}
	}
}
