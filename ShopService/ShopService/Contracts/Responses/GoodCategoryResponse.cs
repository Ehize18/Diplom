using System.Diagnostics.CodeAnalysis;
using ShopService.Core.Entities;

namespace ShopService.Contracts.Responses
{
	public class GoodCategoryResponse
	{
		public Guid Id { get; set; }
		public required string Title { get; set; }
		public string Description { get; set; } = string.Empty;
		public Guid? ParentCategoryId { get; set; }
		public Guid? ImageId { get; set; }
		public List<GoodCategoryResponse> Childs { get; set; } = new List<GoodCategoryResponse>();

		[SetsRequiredMembers]
		public GoodCategoryResponse(GoodCategory goodCategory)
		{
			Id = goodCategory.Id;
			Title = goodCategory.Title;
			Description = goodCategory.Description;
			ParentCategoryId = goodCategory.ParentCategoryId;
			ImageId = goodCategory.ImageId;
			foreach (var child in goodCategory.ChildCategories)
			{
				Childs.Add(new GoodCategoryResponse(child));
			}
		}
	}
}
