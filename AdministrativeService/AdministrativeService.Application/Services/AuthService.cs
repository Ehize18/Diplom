using AdministrativeService.Application.DTO.Auth;
using AdministrativeService.Application.Interfaces;
using AdministrativeService.Core;
using AdministrativeService.Core.Entities;
using AdministrativeService.Database.Interfaces;
using Shared.Jwt;

namespace AdministrativeService.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly IBaseRepository<User> _usersRepository;

		private readonly IJwtProvider _jwtProvider;

		public AuthService(IBaseRepository<User> usersRepository, IJwtProvider jwtProvider)
		{
			_usersRepository = usersRepository;
			_jwtProvider = jwtProvider;
		}

		public async Task<AuthResponseDTO> Authenticate(AuthRequestDTO request)
		{
			///TODO Authentication
			var user = Consts.SystemUser;

			var token = _jwtProvider.GenerateToken(new Dictionary<string, string>
			{
				{ "Id", user.Id.ToString() },
				{ "Username", user.Username }
			});

			var response = new AuthResponseDTO
			{
				Id = user.Id,
				Username = user.Username,
				Token = token
			};

			return response;
		}
	}
}
