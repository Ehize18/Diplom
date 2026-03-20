namespace AdministrativeService.Contracts.User
{
	public class UserResponse
	{
		public Guid Id { get; set; }
		public required string Username { get; set; }
	}
}
