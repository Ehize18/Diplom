using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShopService.Core.Entities
{
	public class DeliveryMethod : BaseEntity
	{
		public required string Title { get; set; }

		[JsonIgnore]
		public string Metadata { get; set; } = "{}";

		[JsonPropertyName("metadata")]
		public Dictionary<string, string>? MetadataBody 
		{
			get
			{
				return JsonSerializer.Deserialize<Dictionary<string, string>>(Metadata);
			}
			set
			{
				Metadata = JsonSerializer.Serialize(value);
			}
		}
	}
}
