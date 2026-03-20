namespace Shared.Jwt
{
	public class JwtOptions
	{
		public required string SecretKey { get; set; }
		public int ExpiredHours { get; set; }
	}
}
