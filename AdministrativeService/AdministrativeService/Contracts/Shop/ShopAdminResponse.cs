using System.Diagnostics.CodeAnalysis;
using AdministrativeService.Contracts.User;
using AdministrativeService.Core.Entities;
using AdministrativeService.Core.Enums;

namespace AdministrativeService.Contracts.Shop
{
	public class ShopAdminResponse : UserResponse
	{
		public AdminFeature Feature { get; set; }

		[SetsRequiredMembers]
		public ShopAdminResponse(ShopAdmin admin)
		{
			Id = admin.UserId;
			Username = admin.User.Username;
			Feature = admin.Feature;
		}
	}
}
