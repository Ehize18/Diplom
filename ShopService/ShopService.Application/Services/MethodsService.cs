using Shared.Enums;
using Shared.RabbitMQ.Contracts;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;

namespace ShopService.Application.Services
{
	public class MethodsService
	{
		private readonly IBaseRepository<DeliveryMethod> _deliveryMethodRepository;

		private readonly IBaseRepository<PaymentMethod> _paymentMethodRepository;

		public MethodsService(IBaseRepository<DeliveryMethod> deliveryMethodRepository, IBaseRepository<PaymentMethod> paymentMethodRepository)
		{
			_deliveryMethodRepository = deliveryMethodRepository;
			_paymentMethodRepository = paymentMethodRepository;
		}

		public async Task<MethodUpdated> CreateMethod(UpdateMethod update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _deliveryMethodRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return new MethodUpdated
				{
					MethodId = Guid.Empty,
					IsSuccess = false,
					Error = "not_admin"
				};
			}

			try
			{
				var methodId = Guid.Empty;
				switch (update.MethodType)
				{
					case MethodType.Delivery:
						var deliveryMethod = await CreateDeliveryMethod(update, cancellationToken);
						methodId = deliveryMethod.Id;
						break;
					case MethodType.Payment:
						var paymentMethod = await CreatePaymentMethod(update, cancellationToken);
						methodId = paymentMethod.Id;
						break;
				}
				return new MethodUpdated
				{
					MethodId = methodId,
					IsSuccess = true
				};
			}
			catch (Exception ex)
			{
			}

			return new MethodUpdated
			{
				MethodId = Guid.Empty,
				IsSuccess = false,
				Error = "unexprected_error"
			};
		}

		private async Task<DeliveryMethod> CreateDeliveryMethod(UpdateMethod update, CancellationToken cancellationToken = default)
		{
			var method = new DeliveryMethod
			{
				Title = update.Title,
				Metadata = update.Metadata
			};
			await _deliveryMethodRepository.SetUser(update.UpdatedById);
			var result = _deliveryMethodRepository.Create(method);
			await _deliveryMethodRepository.SaveChangesAsync(cancellationToken);
			return result;
		}

		private async Task<PaymentMethod> CreatePaymentMethod(UpdateMethod update, CancellationToken cancellationToken = default)
		{
			var method = new PaymentMethod
			{
				Title = update.Title,
				Metadata = update.Metadata
			};
			await _paymentMethodRepository.SetUser(update.UpdatedById);
			var result = _paymentMethodRepository.Create(method);
			await _paymentMethodRepository.SaveChangesAsync(cancellationToken);
			return result;
		}

		public async Task<MethodUpdated> UpdateMethod(UpdateMethod update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _deliveryMethodRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return new MethodUpdated
				{
					MethodId = Guid.Empty,
					IsSuccess = false,
					Error = "not_admin"
				};
			}

			try
			{
				if (update.MethodId == null)
				{
					return new MethodUpdated
					{
						MethodId = Guid.Empty,
						IsSuccess = false,
						Error = "no_method_id"
					};
				}

				switch (update.MethodType)
				{
					case MethodType.Delivery:
						var deliveryMethod = await _deliveryMethodRepository.GetByIdAsync(update.MethodId.Value);
						if (deliveryMethod == null)
						{
							return new MethodUpdated { MethodId = Guid.Empty, IsSuccess = false, Error = "not_found" };
						}
						deliveryMethod.Title = update.Title;
						deliveryMethod.Metadata = update.Metadata;
						await _deliveryMethodRepository.SetUser(update.UpdatedById);
						_deliveryMethodRepository.Update(deliveryMethod);
						await _deliveryMethodRepository.SaveChangesAsync(cancellationToken);
						return new MethodUpdated { MethodId = deliveryMethod.Id, IsSuccess = true };
					case MethodType.Payment:
						var paymentMethod = await _paymentMethodRepository.GetByIdAsync(update.MethodId.Value);
						if (paymentMethod == null)
						{
							return new MethodUpdated { MethodId = Guid.Empty, IsSuccess = false, Error = "not_found" };
						}
						paymentMethod.Title = update.Title;
						paymentMethod.Metadata = update.Metadata;
						await _paymentMethodRepository.SetUser(update.UpdatedById);
						_paymentMethodRepository.Update(paymentMethod);
						await _paymentMethodRepository.SaveChangesAsync(cancellationToken);
						return new MethodUpdated { MethodId = paymentMethod.Id, IsSuccess = true };
				}

				return new MethodUpdated { MethodId = Guid.Empty, IsSuccess = false, Error = "unknown_type" };
			}
			catch (Exception)
			{
			}

			return new MethodUpdated
			{
				MethodId = Guid.Empty,
				IsSuccess = false,
				Error = "unexpected_error"
			};
		}

		public async Task<IList<PaymentMethod>> GetPaymentMethods(CancellationToken cancellationToken = default)
		{
			return await _paymentMethodRepository.GetByPage(1, 100);
		}

		public async Task<IList<DeliveryMethod>> GetDeliveryMethods(CancellationToken cancellationToken = default)
		{
			return await _deliveryMethodRepository.GetByPage(1, 100);
		}
	}
}
