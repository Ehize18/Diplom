namespace Shared.RabbitMQ.Contracts
{
	public class CategoryUpdated
	{
		public required Guid CategoryId { get; set; }
		public required bool IsSuccess { get; set; }
		public string? Error { get; set; }
	}
}
