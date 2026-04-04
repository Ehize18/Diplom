namespace AdministrativeService.Application.DTO.ShopContent
{
	public class PatchPropertyDTO : CreatePropertyDTO
	{
		public required Guid PropertyId { get; set; }
	}
}
