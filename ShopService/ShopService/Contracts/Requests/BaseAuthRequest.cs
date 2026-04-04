namespace ShopService.Contracts.Requests
{
	public record BaseAuthRequest(Guid UserId, string Username);
}
