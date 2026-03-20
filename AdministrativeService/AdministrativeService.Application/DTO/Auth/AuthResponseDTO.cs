namespace AdministrativeService.Application.DTO.Auth
{
	public class AuthResponseDTO
	{
		public Guid Id { get; set; }
		public required string Username { get; set; }
		public required string Token { get; set; }
	}
}
