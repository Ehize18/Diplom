using AdministrativeService.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministrativeService.Controllers
{
	[Authorize]
	public abstract class ControllerWithUser : ControllerBase
	{
		protected User CurrentUser
		{
			get
			{
				return new User
				{
					Id = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value),
					Username = User.Claims.First(c => c.Type == "Username").Value
				};
			}
		}
	}
}
