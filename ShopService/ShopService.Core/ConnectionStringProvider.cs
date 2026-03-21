namespace ShopService.Core
{
	public class ConnectionStringProvider
	{
		public Guid ShopId { get; set; }

		private readonly string _template;

		public string ConnectionString
		{
			get
			{
				return string.Format(_template, ShopId.ToString().ToLower());
			}
		}

		public ConnectionStringProvider(string template)
		{
			_template = template;
		}
	}
}
