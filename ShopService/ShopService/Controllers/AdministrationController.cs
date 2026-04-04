using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopService.Application.Services;
using ShopService.Core;
using ShopService.Core.Requests;
using ShopService.Database;

namespace ShopService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdministrationController : ControllerBase
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly MessageService _messageService;

		public AdministrationController(IServiceProvider serviceProvider, MessageService messageService)
		{
			_serviceProvider = serviceProvider;
			_messageService = messageService;
		}

		[HttpGet("checkShop")]
		public async Task<IActionResult> CheckShopExists(Guid shopId = default, CancellationToken cancellationToken = default)
		{
			var result = false;
			if (shopId == Guid.Empty)
			{
				if (!Guid.TryParse(Request.Headers["X-Shop-Id"], out shopId))
				{
					return Ok(false);
				}
			}

			result = await PingDb(shopId, cancellationToken);

			if (result)
			{
				return Ok(true);
			}
			return Ok(false);
		}

		[HttpGet("vk")]
		public async Task<IActionResult> GetShopByVk(long vkGroupId, CancellationToken cancellationToken = default)
		{
			var result = await _messageService.GetShopByVk(vkGroupId, cancellationToken);
			return Ok(result);
		}

		private async Task<bool> PingDb(Guid shopId, CancellationToken cancellationToken = default)
		{
			try
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var connectionStringProvider = scope.ServiceProvider.GetRequiredService<ConnectionStringProvider>();
					connectionStringProvider.ShopId = shopId;
					var dbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
					var result = await dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
					//await dbContext.Database.MigrateAsync();
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			
		}
	}
}
