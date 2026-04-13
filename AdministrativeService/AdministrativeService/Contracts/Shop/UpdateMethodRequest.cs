namespace AdministrativeService.Contracts.Shop
{
	public record UpdateMethodRequest(string Title, Dictionary<string, string>? Metadata);
}
