using System.Diagnostics.CodeAnalysis;

namespace AdministrativeService.Contracts.Shop
{
	public class ShopResponse
	{
		public Guid Id { get; set; }
		public required string Title { get; set; }
		public string? Description { get; set; }
		public long? VkGroupId { get; set; }
		public List<ShopAdminResponse> Admins { get; set; } = new List<ShopAdminResponse>();

		[SetsRequiredMembers]
		public ShopResponse(AdministrativeService.Core.Entities.Shop shop)
		{
			Id = shop.Id;
			Title = shop.Title;
			Description = shop.Description;
			VkGroupId = shop.VkGroupId;

			if (shop.Admins != null)
			{
				foreach (var admin in shop.Admins)
				{
					Admins.Add(new ShopAdminResponse(admin));
				}
			}
		}
	}
}
