namespace Shared.RabbitMQ.Contracts
{
	public class DataGet
	{
		public required Guid ShopId { get; set; }
		public DataGetEntity Entity { get; set; }
		public string? OrderBy { get; set; }
		public bool IsAscending { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public Filter? Filter { get; set; }
	}

	public class Filter
	{
		public required FilterType FilterType { get; set; }
		public required string LeftExpression { get; set; }
		public required string RightExpression { get; set; }
	}

	public enum DataGetEntity
	{
		Category,
		Good
	}

	public enum FilterType
	{
		Equal,
		NotEqual,
		Less,
		LessOrEqual,
		Greater,
		GreaterOrEqual,
		Like,
		IsNull,
		IsNotNull
	}
}
