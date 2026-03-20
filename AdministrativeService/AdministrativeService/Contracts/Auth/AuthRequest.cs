using AdministrativeService.Application.DTO.Auth;

namespace AdministrativeService.Contracts.Auth
{
	public record AuthRequest
	{
		public AuthRequestDTO ToDTO()
		{
			return new AuthRequestDTO();
		}
	}
}
