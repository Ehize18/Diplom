using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ShopService.Filters
{
	public class ShopHeaderParameterOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			if (operation.Parameters == null)
			{
				operation.Parameters = new List<IOpenApiParameter>();
			}

			operation.Parameters.Add(new OpenApiParameter
			{
				Name = "X-Shop-Id",
				In = ParameterLocation.Header,
				Description = "Shop guid",
				Required = false,
				Schema = new OpenApiSchema
				{
					Type = JsonSchemaType.String,
					Default = "019d1022-0d45-73dd-ac5f-cdd9c177d3d5"
				}
			});
		}
	}
}
