namespace ShopService.Core.Entities
{
	/// <summary>
	/// User basket.
	/// </summary>
	public class Basket : BaseEntity
	{
		/// <summary>
		/// Flag is current basket.
		/// </summary>
		public bool IsCurrent { get; set; }

		/// <summary>
		/// Goods in basket.
		/// </summary>
		public List<GoodInBasket> Goods { get; set; } = new List<GoodInBasket>();

	}
}
