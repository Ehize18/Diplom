using Shared.RabbitMQ.Contracts;
using ShopService.Core.Entities;
using ShopService.Database.Interfaces;
using ShopService.Database.Repositories;

namespace ShopService.Application.Services
{
	public class PropertyService
	{
		private readonly IGoodPropertyRepository _goodPropertyRepository;

		public PropertyService(IGoodPropertyRepository goodPropertyRepository)
		{
			_goodPropertyRepository = goodPropertyRepository;
		}

		public async Task<GoodPropertyCategory?> CreateGoodProperty(UpdateProperty update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _goodPropertyRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return null;
			}

			try
			{
				var property = new GoodPropertyCategory
				{
					Title = update.PropertyTitle
				};
				await _goodPropertyRepository.SetUser(update.UpdatedById);
				var result = _goodPropertyRepository.Create(property);
				await _goodPropertyRepository.SaveChangesAsync(cancellationToken);
				return result;
			}
			catch (Exception ex)
			{
			}

			return null;
		}

		public async Task<GoodPropertyCategory?> UpdateGoodProperty(UpdateProperty update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _goodPropertyRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return null;
			}

			try
			{
				await _goodPropertyRepository.SetUser(update.UpdatedById);
				var property = await _goodPropertyRepository.GetByIdAsync((Guid)update.PropertyId!);
				if (property == null)
				{
					return null;
				}
				property.Title = update.PropertyTitle;
				var result = _goodPropertyRepository.Update(property);
				await _goodPropertyRepository.SaveChangesAsync(cancellationToken);
				return result;
			}
			catch { }

			return null;
		}

		public async Task<GoodProperty?> CreateGoodPropertyValue(UpdatePropertyValue update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _goodPropertyRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return null;
			}

			try
			{
				var propertyValue = new GoodProperty
				{
					Title = update.PropertyTitle,
					CategoryId = (Guid)update.PropertyId!
				};
				await _goodPropertyRepository.SetUser(update.UpdatedById);
				var result = _goodPropertyRepository.Create(propertyValue);
				await _goodPropertyRepository.SaveChangesAsync(cancellationToken);
				return result;
			}
			catch (Exception ex)
			{
			}

			return null;
		}

		public async Task<GoodProperty?> UpdateGoodPropertyValue(UpdatePropertyValue update, CancellationToken cancellationToken = default)
		{
			var isAdmin = await _goodPropertyRepository.CheckOrCreateAdmin(update.UpdatedById);
			if (!isAdmin)
			{
				return null;
			}

			try
			{
				await _goodPropertyRepository.SetUser(update.UpdatedById);
				var propertyValue = await _goodPropertyRepository.GetPropertyValueById((Guid)update.PropertyValueId!);
				if (propertyValue == null)
				{
					return null;
				}
				propertyValue.Title = update.PropertyTitle;
				var result = _goodPropertyRepository.Update(propertyValue);
				await _goodPropertyRepository.SaveChangesAsync(cancellationToken);
				return result;
			}
			catch { }

			return null;
		}
	}
}
