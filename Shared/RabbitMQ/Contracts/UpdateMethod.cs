using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.Enums;

namespace Shared.RabbitMQ.Contracts
{
	public class UpdateMethod
	{
		public required UpdateType UpdateType { get; set; }
		public required MethodType MethodType { get; set; }
		public Guid? MethodId { get; set; }
		public required string Title { get; set; }
		public string Metadata { get; set; } = "{}";

		public required Guid UpdatedById { get; set; }
		public required Guid ShopId { get; set; }

		[JsonIgnore]
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
