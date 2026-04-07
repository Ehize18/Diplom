using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopService.Application.Services;

namespace ShopService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MethodsController : ControllerBase
	{
		private readonly MethodsService _methodsService;

		public MethodsController(MethodsService methodsService)
		{
			_methodsService = methodsService;
		}

		[HttpGet("payment")]
		public async Task<ActionResult> GetPaymentMethods(CancellationToken cancellationToken)
		{
			var result = await _methodsService.GetPaymentMethods(cancellationToken);
			if (result == null)
			{
				return BadRequest();
			}
			return Ok(result);
		}

		[HttpGet("delivery")]
		public async Task<ActionResult> GetDeliveryMethods(CancellationToken cancellationToken)
		{
			var result = await _methodsService.GetDeliveryMethods(cancellationToken);
			if (result == null)
			{
				return BadRequest();
			}
			return Ok(result);
		}
	}
}
