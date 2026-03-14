using ShopService.Core.Entities;

namespace ShopService.Core
{
	/// <summary>
	/// Constants.
	/// </summary>
	public static class Consts
	{
		/// <summary>
		/// System user.
		/// </summary>
		public static readonly User SystemUser = new User
		{
			Id = new Guid("765795f6-ef64-4d68-a517-cfe5208a3e01"),
			Username = "SYSTEM",
			CreatedById = new Guid("765795f6-ef64-4d68-a517-cfe5208a3e01")
		};
	}
}
