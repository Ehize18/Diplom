using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Jwt
{
	public interface IJwtProvider
	{
		string GenerateToken(IDictionary<string, string> claims);
		string GenerateToken(IEnumerable<Claim> claims);
	}

	public class JwtProvider : IJwtProvider
	{
		private readonly JwtOptions _jwtOptions;

		public JwtProvider(IOptions<JwtOptions> options)
		{
			_jwtOptions = options.Value;
		}

		public string GenerateToken(IDictionary<string, string> claims)
		{
			var claimList = new List<Claim>();

			foreach (var claim in claims)
			{
				claimList.Add(new(claim.Key, claim.Value));
			}

			return GenerateToken(claimList);
		}

		public string GenerateToken(IEnumerable<Claim> claims)
		{
			var signingCredentials = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				claims: claims,
				signingCredentials: signingCredentials,
				expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpiredHours));

			var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

			return tokenValue;
		}
	}
}
