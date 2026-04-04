namespace ShopService.Core.Entities
{
	public class GoodCategoriesAll
	{
		public Guid FilterCategoryId { get; set; }

		public Guid GoodId { get; set; }

		public Guid ActualCategoryId { get; set; }

		public Good? Good { get; set; }
		public GoodCategory? FilterCategory { get; set; }
		public GoodCategory? ActualCategory { get; set; }
	}
}
