using System.Text.Json;
using System.Xml.Linq;
using AdministrativeService.Application.DTO.ShopContent;
using AdministrativeService.Application.Services;
using AdministrativeService.Contracts.Content;
using AdministrativeService.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;
using Shared.RabbitMQ.Contracts;
using Shared.S3;

namespace AdministrativeService.Controllers
{
	[Route("api/[controller]/{shopId:guid}")]
	[ApiController]
	[Authorize]
	public class ShopContentController : ControllerWithUser
	{
		private readonly ShopContentService _shopContentService;
		private readonly IMinioService _minioService;
		private readonly string? _domain;

		public ShopContentController(ShopContentService shopContentService, IMinioService minioService, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			_shopContentService = shopContentService;
			_minioService = minioService;
			_domain = configuration.GetValue<string>("ImageDomain");
		}

		[HttpPost("category")]
		public async Task<ActionResult> CreateCategory([FromForm]CreateCategoryRequest request, IFormFile? image, Guid shopId, CancellationToken cancellationToken = default)
		{
			Guid? imageId = null;
			var tasksToWait = new List<Task>();
			if (image != null)
			{
				imageId = Guid.NewGuid();
				tasksToWait.Add(UploadImage((Guid)imageId, shopId, image, cancellationToken));
			}
			var createCategoryTask = _shopContentService.CreateCategory(new()
			{
				ShopId = shopId,
				Title = request.Title,
				Description = request.Description,
				ParentCategoryId = request.ParentCategoryId,
				IsActive = request.IsActive,
				User = CurrentUser,
				ImageId = imageId,
			}, cancellationToken);
			tasksToWait.Add(createCategoryTask);

			await Task.WhenAll(tasksToWait);

			var (id, error) = createCategoryTask.Result;

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("category/{categoryId:guid}")]
		public async Task<ActionResult> PutCategory([FromForm]UpdateCategoryRequest request, IFormFile? image, Guid shopId, Guid categoryId, CancellationToken cancellationToken = default)
		{
			Guid? imageId = request.ImageId;
			var tasksToWait = new List<Task>();
			if (image != null)
			{
				imageId = Guid.NewGuid();
				tasksToWait.Add(UploadImage((Guid)imageId, shopId, image, cancellationToken));
			}
			Guid? parentCategoryId = null;
			if (Guid.TryParse(request.ParentCategoryId, out var parentCategoryIdParsed))
			{
				if (parentCategoryIdParsed != Guid.Empty)
				{
					parentCategoryId = parentCategoryIdParsed;
				}
			}
			var patchTask = _shopContentService.PatchCategory(new()
			{
				ShopId = shopId,
				CategoryId = categoryId,
				Title = request.Title,
				Description = request.Description,
				IsActive = request.IsActive,
				ParentCategoryId = parentCategoryId,
				ImageId = imageId,
				User = CurrentUser
			});
			tasksToWait.Add(patchTask);
			await Task.WhenAll(tasksToWait);
			var (id, error) = patchTask.Result;

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpDelete("category/{categoryId:guid}")]
		public async Task<ActionResult> DeleteCategory(Guid shopId, Guid categoryId, string? imageId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.DeleteCategory(new()
			{
				ShopId = shopId,
				CategoryId = categoryId,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				if (!string.IsNullOrWhiteSpace(imageId) && Guid.TryParse(imageId, out var imageGuid))
				{
					await _minioService.DeleteObject(shopId.ToString(), imageId, cancellationToken);
				}
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPost("good")]
		public async Task<ActionResult> CreateGood([FromForm]CreateGoodRequest request, IFormFile? image, Guid shopId, CancellationToken cancellationToken = default)
		{
			Guid? imageId = null;
			var tasksToWait = new List<Task>();
			if (image != null)
			{
				imageId = Guid.NewGuid();
				tasksToWait.Add(UploadImage((Guid)imageId, shopId, image, cancellationToken));
			}

			var createGoodTask = _shopContentService.CreateGood(new()
			{
				ShopId = shopId,
				Title = request.Title,
				Description = request.Description,
				CategoryId = request.CategoryId,
				Count = request.Count,
				Price = request.Price,
				OldPrice = request.OldPrice,
				ImageId = imageId,
				User = CurrentUser
			});

			await Task.WhenAll(tasksToWait);

			var (id, error) = createGoodTask.Result;

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("good/{goodId:guid}")]
		public async Task<ActionResult> PutGood([FromForm]UpdateGoodRequest request, IFormFile? image, Guid shopId, Guid goodId, CancellationToken cancellationToken = default)
		{
			Guid? imageId = request.ImageId;
			var tasksToWait = new List<Task>();
			if (image != null)
			{
				imageId = Guid.NewGuid();
				tasksToWait.Add(UploadImage((Guid)imageId, shopId, image, cancellationToken));
			}
			Guid? categoryId = null;
			if (Guid.TryParse(request.CategoryId, out var categoryIdParsed))
			{
				if (categoryIdParsed != Guid.Empty)
				{
					categoryId = categoryIdParsed;
				}
			}
			var patchTask = _shopContentService.PatchGood(new()
			{
				ShopId = shopId,
				GoodId = goodId,
				Title = request.Title,
				Description = request.Description,
				CategoryId = categoryId,
				Count = request.Count,
				Price = request.Price,
				OldPrice = request.OldPrice,
				ImageId = imageId,
				User = CurrentUser
			});
			tasksToWait.Add(patchTask);
			await Task.WhenAll(tasksToWait);
			var (id, error) = patchTask.Result;

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpDelete("good/{goodId:guid}")]
		public async Task<ActionResult> DeleteGood(Guid shopId, Guid goodId, string? imageId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.DeleteGood(new()
			{
				ShopId = shopId,
				GoodId = goodId,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				if (!string.IsNullOrWhiteSpace(imageId) && Guid.TryParse(imageId, out var imageGuid))
				{
					await _minioService.DeleteObject(shopId.ToString(), imageId, cancellationToken);
				}
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("order/{orderId:guid}")]
		public async Task<ActionResult> PutOrder([FromBody]UpdateOrderRequest request, Guid shopId, Guid orderId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.PatchOrder(new()
			{
				ShopId = shopId,
				OrderId = orderId,
				EntityType = request.EntityType,
				StatusValue = request.StatusValue,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpGet("{entityType}")]
		public async Task<ActionResult> GetData(string entityType,
			Guid shopId, string? orderBy, bool? isAscending, int? page, int? pageSize,
			FilterType? filterType, string? column, string? columnValue,
			CancellationToken cancellationToken = default)
		{
			if (Enum.TryParse<DataGetEntity>(entityType, true, out var type))
			{
				var data = await GetData(type, shopId, orderBy, isAscending,
					page, pageSize, filterType, column, columnValue, cancellationToken);

				if (data == null)
				{
					return BadRequest();
				}
				return Ok(data);
			}
			return BadRequest("unknown_type");
		}

		private async Task<DataGetResponse?> GetData(DataGetEntity entityType,
			Guid shopId, string? orderBy, bool? isAscending, int? page, int? pageSize,
			FilterType? filterType, string? column, string? columnValue,
			CancellationToken cancellationToken = default)
		{
			var dto = new GetDataDTO
			{
				Entity = entityType,
				ShopId = shopId
			};

			if (orderBy != null)
			{
				dto.OrderBy = orderBy;
				dto.IsAscending = isAscending != null ? (bool)isAscending : false;
			}

			if (page != null && page > 0 && pageSize != null && pageSize > 0)
			{
				dto.Page = (int)page;
				dto.PageSize = (int)pageSize;
			}

			if (filterType != null && !string.IsNullOrWhiteSpace(column))
			{
				dto.Filter = new Filter
				{
					FilterType = (FilterType)filterType,
					LeftExpression = column,
					RightExpression = columnValue
				};
			}

			var response = await _shopContentService.GetData(dto, cancellationToken);

			return response;
		}

		[HttpGet("export/{entityType}")]
		public async Task<ActionResult> ExportData(string entityType,
			Guid shopId, string? orderBy, bool? isAscending, int? page, int? pageSize,
			FilterType? filterType, string? column, string? columnValue,
			CancellationToken cancellationToken = default)
		{
			if (Enum.TryParse<DataGetEntity>(entityType, true, out var type))
			{
				var data = await GetData(type, shopId, orderBy, isAscending,
					page, pageSize, filterType, column, columnValue, cancellationToken);

				if (data == null)
				{
					return BadRequest();
				}

				var fileName = $"export_{entityType}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

				var config = new OpenXmlConfiguration
				{
					AutoFilter = false,
					EnableAutoWidth = false,
					FastMode = true,

				};

				Stream ms;

				switch (type)
				{
					case DataGetEntity.Order:
						ms = await GetOrderExcelStream(data, config, cancellationToken);
						break;
					default:
						ms = await GetDefaultExcelStream(shopId, data, config, cancellationToken);
						break;
				}

				return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
			}
			return BadRequest("unknown_type");
		}

		private async Task<MemoryStream> GetOrderExcelStream(
			DataGetResponse data,
			OpenXmlConfiguration config,
			CancellationToken cancellationToken)
		{
			var ms = new MemoryStream();

			var orders = new List<Dictionary<string, object>>();
			var goodElements = new Dictionary<string, List<JsonElement>>();

			foreach (var obj in data.Results)
			{
				if (obj is not JsonElement orderElement) continue;

				JsonElement? basketElement = null;
				if (orderElement.TryGetProperty("basket", out var be) && be.ValueKind == JsonValueKind.Object)
					basketElement = be;

				var orderDict = ConvertJsonElement(orderElement, exclude: new[] { "basket" });
				var orderId = orderDict.GetValueOrDefault("id")?.ToString();
				if (orderDict.ContainsKey("deliveryMethod") && orderDict["deliveryMethod"] is Dictionary<string, object> deliveryMethod)
				{
					orderDict["deliveryMethod"] = deliveryMethod["title"];
				}
				if (orderDict.ContainsKey("paymentMethod") && orderDict["paymentMethod"] is Dictionary<string, object> paymentMethod)
				{
					orderDict["paymentMethod"] = paymentMethod["title"];
				}
				orders.Add(orderDict);
				goodElements[orderId] = new List<JsonElement>();

				if (basketElement.HasValue)
				{
					var basketDict = ConvertJsonElement(basketElement.Value, exclude: new[] { "goods" });

					if (basketElement.Value.TryGetProperty("goods", out var goodsArray)
						&& goodsArray.ValueKind == JsonValueKind.Array)
					{
						goodElements[orderId].Add(goodsArray);
					}
				}
			}

			var goods = goodElements.SelectMany(SelectGoods);

			// 5. Формируем словарь листов (явный тип!)
			var sheets = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
			{
				["Order"] = orders,
				["Goods"] = goods
			};

			await MiniExcel.SaveAsAsync(ms, sheets, configuration: config, cancellationToken: cancellationToken);
			ms.Position = 0;
			return ms;
		}

		private IEnumerable<IDictionary<string, object>> SelectGoods(KeyValuePair<string, List<JsonElement>> elements)
		{
			foreach (var goodsArray in elements.Value)
			{
				foreach (var good in goodsArray.EnumerateArray())
				{
					var goodDict = ConvertJsonElement(good);
					if (goodDict["good"] == null)
					{
						goodDict["good"] = "Удалено";
					}
					else
					{
						goodDict["good"] = ((Dictionary<string, object>)goodDict["good"])["title"];
					}
					goodDict["OrderId"] = elements.Key;
					goodDict.Remove("basketId");
					yield return goodDict;
				}
			}
		}

		private async Task<MemoryStream> GetDefaultExcelStream(Guid shopId, DataGetResponse data, OpenXmlConfiguration config, CancellationToken cancellationToken)
		{
			var ms = new MemoryStream();

			var dictData = data.Results.Select(obj =>
			{
				if (obj is JsonElement element)
				{
					var dict = ConvertJsonElement(element);
					if (dict.ContainsKey("imageId"))
					{
						dict["imageUrl"] = $"{_domain}/{shopId}/{dict["imageId"]}";
					}
					return dict;
				}
				return new Dictionary<string, object>();
			});

			await MiniExcel.SaveAsAsync(ms, dictData, configuration: config, cancellationToken: cancellationToken);
			ms.Position = 0;
			return ms;
		}

		private static Dictionary<string, object> ConvertJsonElement(JsonElement element, string[]? exclude = null)
		{
			var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			var excludeSet = exclude?.ToHashSet(StringComparer.OrdinalIgnoreCase);

			if (element.ValueKind != JsonValueKind.Object)
				return result;

			foreach (var prop in element.EnumerateObject())
			{
				if (excludeSet?.Contains(prop.Name) == true) continue;

				result[prop.Name] = ConvertValue(prop.Value);
			}
			return result;
		}

		private static object? ConvertValue(JsonElement value)
		{
			return value.ValueKind switch
			{
				JsonValueKind.Null => null,
				JsonValueKind.String => value.GetString(),
				JsonValueKind.Number => TryGetNumber(value),
				JsonValueKind.True => true,
				JsonValueKind.False => false,

				// Массив простых значений → склеиваем
				JsonValueKind.Array when IsSimpleArray(value)
					=> string.Join(", ", value.EnumerateArray()
						.Select(ConvertValue)
						.Where(x => x != null)),

				// Массив объектов или вложенный объект → рекурсивная конвертация в Dictionary
				JsonValueKind.Array => value.EnumerateArray()
					.Select(ConvertValue)
					.Where(x => x != null)
					.ToList(), // List<object> MiniExcel обработает как строку "[...]"

				JsonValueKind.Object => ConvertJsonElement(value), // Рекурсия

				_ => value.GetRawText() // Fallback: строковое представление
			};
		}

		// 🔹 Вспомогательные методы
		private static object? TryGetNumber(JsonElement value)
		{
			try
			{
				return value.GetDecimal();
			}
			catch
			{
				return value.GetDouble();
			}
		}

		private static bool IsSimpleArray(JsonElement array)
		{
			return array.EnumerateArray()
				.All(e => e.ValueKind is JsonValueKind.String
								 or JsonValueKind.Number
								 or JsonValueKind.Null
								 or JsonValueKind.True
								 or JsonValueKind.False);
		}

		[HttpPost("property")]
		public async Task<ActionResult> CreateProperty(CreatePropertyRequest request, Guid shopId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.CreateProperty(new()
			{
				ShopId = shopId,
				Title = request.Title,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("property/{propertyId:guid}")]
		public async Task<ActionResult> UpdateProperty(CreatePropertyRequest request, Guid shopId, Guid propertyId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.UpdateProperty(new()
			{
				PropertyId = propertyId,
				ShopId = shopId,
				Title = request.Title,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPost("property/{propertyId:guid}")]
		public async Task<ActionResult> CreatePropertyValue(CreatePropertyRequest request, Guid shopId, Guid propertyId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.CreatePropertyValue(new()
			{
				PropertyId = propertyId,
				ShopId = shopId,
				Title = request.Title,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPut("property/{propertyId:guid}/{propertyValueId:guid}")]
		public async Task<ActionResult> UpdateProperty(CreatePropertyRequest request, Guid shopId, Guid propertyId, Guid propertyValueId, CancellationToken cancellationToken = default)
		{
			var (id, error) = await _shopContentService.UpdatePropertyValue(new()
			{
				PropertyValueId = propertyValueId,
				ShopId = shopId,
				Title = request.Title,
				User = CurrentUser
			}, cancellationToken);

			if (string.IsNullOrWhiteSpace(error))
			{
				return Ok(id);
			}
			return BadRequest(error);
		}

		[HttpPost("image")]
		public async Task<ActionResult> UploadImage(Guid shopId, IFormFile image, CancellationToken cancellationToken = default)
		{
			var imageId = Guid.NewGuid();

			await UploadImage(imageId, shopId, image, cancellationToken);

			return Ok(imageId);
		}

		private async Task UploadImage(Guid imageId, Guid shopId, IFormFile image, CancellationToken cancellationToken = default)
		{
			var policyJson = @"{
				""Version"": ""2012-10-17"",
				""Statement"": [{
					""Effect"": ""Allow"",
					""Principal"": { ""AWS"": ""*"" },
					""Action"": [""s3:GetObject""],
					""Resource"": [""arn:aws:s3:::" + shopId.ToString() + @"/*""]
				}]
			}";
			await _minioService.Init(shopId.ToString(), policyJson, cancellationToken);

			using (var stream = new MemoryStream())
			{
				await image.CopyToAsync(stream);
				stream.Position = 0;
				await _minioService.PutObject(shopId.ToString(), imageId.ToString(), stream, image.ContentType, cancellationToken);
			}
		}

		[HttpDelete("image/{imageId:guid}")]
		public async Task<ActionResult> DeleteImage(Guid shopId, Guid imageId, CancellationToken cancellationToken = default)
		{
			await _minioService.DeleteObject(shopId.ToString(), imageId.ToString(), cancellationToken);
			return Ok();
		}
	}
}
