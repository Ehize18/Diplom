using AdministrativeService.Application.Interfaces;
using AdministrativeService.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;

namespace AdministrativeService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost]
		public async Task<IActionResult> Authenticate(AuthRequest request)
		{
			var response = await _authService.Authenticate(request.ToDTO());

			if (response == null)
			{
				return BadRequest();
			}
			else
			{
				Response.Cookies.Append("AdminToken", response.Token, new CookieOptions
				{
					HttpOnly = true
				});
				return Ok(new AuthResponse(response));
			}
		}
	}
}
