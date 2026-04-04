using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopService.Core.Entities;

namespace ShopService.Controllers
{
	[Authorize]
	public class ControllerWithUser : ControllerBase
	{
		protected User CurrentUser
		{
			get
			{
				return new User
				{
					Id = Guid.Parse(User.Claims.First(c => c.Type == "UserId").Value),
					Username = User.Claims.First(c => c.Type == "Username").Value
				};
			}
		}
	}
}
