namespace Shared.RabbitMQ.Contracts
{
	public class DataGetResponse
	{
		public object[] Results { get; set; } = [];
		public bool IsSuccess { get; set; } = false;
		public string Error { get; set; } = string.Empty;
	}
}
