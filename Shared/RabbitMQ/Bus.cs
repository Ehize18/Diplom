namespace Shared.RabbitMQ
{
	public static class Bus
	{
		public static class AdminEvents
		{
			public const string EXCHANGE = "admin-events";

			public const string SHOP_CREATE = "shop.create";
			public const string SHOP_CREATED = "shop.created";
			public const string SHOP_UPDATE = "shop.update";
			public const string SHOP_UPDATED = "shop.updated";
		}

		public static class ShopEvents
		{
			public const string EXCHANGE = "shop-events";

			public const string CATEGORY_UPDATE = "category.update";
			public const string CATEGORY_UPDATED = "category.updated";
			public const string GOOD_UPDATE = "good.update";
		}

		public static class DataBus
		{
			public const string EXCHANGE = "data-bus";

			public const string DATA_GET = "data.get";
			public const string DATA_GET_RESPONSE = "data.get.response";
		}
	}
}
