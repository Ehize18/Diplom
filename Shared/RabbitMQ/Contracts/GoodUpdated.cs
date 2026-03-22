namespace Shared.RabbitMQ.Contracts
{
	public class GoodUpdated
	{
		public required Guid GoodId { get; set; }
		public required bool IsSuccess { get; set; }
		public string? Error { get; set; }
	}
}
