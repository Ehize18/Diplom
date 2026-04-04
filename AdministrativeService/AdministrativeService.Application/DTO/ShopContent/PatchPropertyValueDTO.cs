namespace AdministrativeService.Application.DTO.ShopContent
{
	public class PatchPropertyValueDTO : CreatePropertyDTO
	{
		public required Guid PropertyValueId { get; set; }
	}
}
