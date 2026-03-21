namespace ShopService.Core.Entities
{
	/// <summary>
	/// User.
	/// </summary>
	public class User : BaseEntity
	{
		public required string Username { get; set; }

		public string? PasswordHash { get; set; }

		public int? TelegramId { get; set; }

		public bool IsAdmin { get; set; } = false;

		public List<Basket> Baskets { get; set; } = new List<Basket>();
	}
}
