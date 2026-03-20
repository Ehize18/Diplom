using AdministrativeService.Application.DTO.Auth;

namespace AdministrativeService.Application.Interfaces
{
	public interface IAuthService
	{
		Task<AuthResponseDTO> Authenticate(AuthRequestDTO request);
	}
}