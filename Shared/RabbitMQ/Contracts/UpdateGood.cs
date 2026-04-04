namespace Shared.RabbitMQ.Contracts
{
	public class UpdateGood
	{
		public Guid? GoodId { get; set; }
		public required UpdateType UpdateType { get; set; }
		public required string GoodTitle { get; set; }
		public string? GoodDescription { get; set; }
		public Guid CategoryId { get; set; }
		public Guid? ImageId { get; set; }
		public int Count { get; set; }
		public decimal Price { get; set; }
		public decimal OldPrice { get; set; }
		public required Guid UpdatedById { get; set; }
		public required Guid ShopId { get; set; }
	}
}
