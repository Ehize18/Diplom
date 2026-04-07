namespace Shared.RabbitMQ.Contracts
{
	public class MethodUpdated
	{
		public required Guid MethodId { get; set; }
		public required bool IsSuccess { get; set; }
		public string? Error { get; set; }
	}
}
