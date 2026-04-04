using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Jwt;
using ShopService.Application.Services;
using ShopService.Contracts.Requests;

namespace ShopService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserService _userService;
		private readonly IJwtProvider _jwtProvider;

		public AuthController(UserService userService, IJwtProvider jwtProvider)
		{
			_userService = userService;
			_jwtProvider = jwtProvider;
		}

		[HttpPost("base")]
		public async Task<ActionResult> BaseAuth(BaseAuthRequest request, CancellationToken cancellationToken = default)
		{
			var shopIdHeader = HttpContext.Request.Headers["X-Shop-Id"];

			if (!Guid.TryParse(shopIdHeader, out var shopId))
			{
				return Ok(false);
			}

			var user = await _userService.GetUserById(request.UserId, cancellationToken);

			if (user == null)
			{
				user = await _userService.CreateUser(request.Username, cancellationToken);
			}

			var token = _jwtProvider.GenerateToken(new Dictionary<string, string>()
			{
				{ "UserId", user.Id.ToString() },
				{ "Username", user.Username }
			});

			Response.Cookies.Append(shopIdHeader + "Token", token, new CookieOptions
			{
				HttpOnly = true
			});
			return Ok(true);
		}

		[HttpPost("vk")]
		public async Task<ActionResult> VkAuth(VkAuthRequest request, CancellationToken cancellationToken = default)
		{
			if (!CheckVk(request))
			{
				return Ok(false);
			}

			var shopIdHeader = HttpContext.Request.Headers["X-Shop-Id"];

			if (!Guid.TryParse(shopIdHeader, out var shopId))
			{
				return Ok(false);
			}

			var user = await _userService.GetUserByVkId(request.Id, cancellationToken);

			if (user == null)
			{
				var username = string.Join(' ', request.FirstName, request.LastName);
				user = await _userService.CreateUser(username, request.Id, cancellationToken);
			}

			var token = _jwtProvider.GenerateToken(new Dictionary<string, string>()
			{
				{ "UserId", user.Id.ToString() },
				{ "Username", user.Username }
			});

			Response.Cookies.Append(shopIdHeader + "Token", token, new CookieOptions
			{
				HttpOnly = true,
				SameSite = SameSiteMode.None,
				Secure = true
			});
			return Ok(true);
		}

		private bool CheckVk(VkAuthRequest request)
		{
			var sortedParams = new SortedDictionary<string, string>();
			sortedParams.Add("app_id", (54487683).ToString());
			sortedParams.Add("request_id", $"{request.Id}");
			sortedParams.Add("ts", (request.Ts).ToString());
			sortedParams.Add("user_id", (request.Id).ToString());

			string signParamsQuery = string.Join("&",
				sortedParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

			byte[] keyBytes = Encoding.UTF8.GetBytes("");
			byte[] messageBytes = Encoding.UTF8.GetBytes(signParamsQuery);

			string computedSign;

			using (var hmac = new HMACSHA256(keyBytes))
			{
				byte[] hashBytes = hmac.ComputeHash(messageBytes);

				string base64 = Convert.ToBase64String(hashBytes);

				computedSign = base64
					.Replace('+', '-')
					.Replace('/', '_')
					.TrimEnd('=');
			}

			bool isValid = CryptographicOperations.FixedTimeEquals(
				Encoding.UTF8.GetBytes(computedSign),
				Encoding.UTF8.GetBytes(request.Sign));
			return isValid;
		}
	}
}
