using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopService.Core.Requests;
using ShopService.Database;

namespace ShopService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdministrationController : ControllerBase
	{
		private readonly ShopDbContext _dbContext;

		public AdministrationController(ShopDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpPost("migrate")]
		public IActionResult Migrate()
		{
			if (_dbContext.Database.GetPendingMigrations().Any())
			{
				_dbContext.Database.Migrate();
			}
			return Ok();
		}
	}
}
