using System.Diagnostics.CodeAnalysis;
using AdministrativeService.Application.DTO.Auth;

namespace AdministrativeService.Contracts.Auth
{
	public class AuthResponse
	{
		public Guid Id { get; set; }
		public required string Username { get; set; }
		public required string Token { get; set; }

		[SetsRequiredMembers]
		public AuthResponse(AuthResponseDTO authResponseDTO)
		{
			Id = authResponseDTO.Id;
			Username = authResponseDTO.Username;
			Token = authResponseDTO.Token;
		}
	}
}
