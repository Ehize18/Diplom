using Shared.RabbitMQ.Contracts;

namespace AdministrativeService.Application.DTO.ShopContent
{
	public class GetCategoriesDTO
	{
		public required Guid ShopId { get; set; }
		public required DataGetEntity Entity { get; set; }
		public string? OrderBy { get; set; }
		public bool IsAscending { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public Filter? Filter { get; set; }
	}
}
