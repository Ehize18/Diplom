namespace ShopService.Contracts.Requests
{
	public record VkAuthRequest(string FirstName, string LastName, long Id, string Sign, int Ts);
}
