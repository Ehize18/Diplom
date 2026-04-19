using AdministrativeService.Application.DTO.Shop;
using AdministrativeService.Application.Services;
using AdministrativeService.Contracts.Shop;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.RabbitMQ.Contracts;
using Shared.S3;
using System.IO;
using System.Text.Json;

namespace AdministrativeService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShopController : ControllerWithUser
	{
		private readonly ShopService _shopService;
		private readonly IMinioService _minioService;
		private readonly string? _domain;

		public ShopController(ShopService shopService, IMinioService minioService, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			_shopService = shopService;
			_minioService = minioService;
			_domain = configuration.GetValue<string>("ImageDomain");
		}

		[HttpPost]
		public async Task<ActionResult<ShopResponse>> CreateShop(CreateShopRequest request, CancellationToken cancellationToken)
		{
			var dto = new CreateShopDTO
			{
				Title = request.Title,
				User = CurrentUser
			};
			var shop = await _shopService.CreateShopAsync(dto, cancellationToken);

			if (shop == null)
			{
				return BadRequest();
			}
			return Ok(new ShopResponse(shop));
		}

		[HttpPost("{shopId:guid}/vk")]
		public async Task<ActionResult<ShopResponse>> SetVkGroup(Guid shopId, SetVkGroupRequest request, CancellationToken cancellationToken)
		{
			var shop = await _shopService.UpdateShop(CurrentUser, shopId, request.VkGroupId, cancellationToken);

			if (shop == null)
			{
				return BadRequest();
			}
			return Ok(new ShopResponse(shop));
		}

		[HttpGet]
		public async Task<ActionResult<List<ShopResponse>>> GetUserShops()
		{
			var shops = await _shopService.GetUserShopsAsync(CurrentUser.Id);

			var response = new List<ShopResponse>();

			foreach (var shop in shops)
			{
				response.Add(new(shop));
			}

			return Ok(response);
		}

		[HttpPost("{shopId:guid}/deliverymethod")]
		public async Task<ActionResult> CreateDeliveryMethod(CreateDeliveryMethodRequest request, Guid shopId, CancellationToken cancellationToken)
		{
			MethodUpdated? result = null;
			switch (request.DeliveryType)
			{
				case DeliveryType.Pickup:
					result = await _shopService.CreateDeliveryMethod(CurrentUser, shopId, request.Title, request.Metadata, cancellationToken);
					break;
				case DeliveryType.Post:
					var meta = request.Metadata;
					if (meta == null)
					{
						meta = new Dictionary<string, string>();
					}
					if (!meta.ContainsKey("address_needed"))
					{
						meta.Add("address_needed", "true");
					}
					result = await _shopService.CreateDeliveryMethod(CurrentUser, shopId, request.Title, meta, cancellationToken);
					break;
				default:
					return BadRequest("method_not_found");
			}

			if (result == null)
			{
				return BadRequest();
			}
			if (!result.IsSuccess)
			{
				return BadRequest(result.Error);
			}
			return Ok(result.MethodId);
		}

		[HttpPost("{shopId:guid}/paymentmethod")]
		public async Task<ActionResult> CreatePaymentMethod(CreatePaymentMethodRequest request, Guid shopId, CancellationToken cancellationToken)
		{
			MethodUpdated? result = null;
			var meta = request.Metadata;
			if (meta == null)
			{
				meta = new Dictionary<string, string>();
			}
			switch (request.PaymentType)
			{
				case PaymentType.OnDelivered:
					if (!meta.ContainsKey("payment_info_needed"))
					{
						meta.Add("payment_info_needed", "false");
					}
					result = await _shopService.CreatePaymentMethod(CurrentUser, shopId, request.Title, meta, cancellationToken);
					break;
				case PaymentType.Transfer:
					if (!meta.ContainsKey("payment_info_needed"))
					{
						meta.Add("payment_info_needed", "false");
					}
					result = await _shopService.CreatePaymentMethod(CurrentUser, shopId, request.Title, meta, cancellationToken);
					break;
				default:
					return BadRequest("method_not_found");
			}

			if (result == null)
			{
				return BadRequest();
			}
			if (!result.IsSuccess)
			{
				return BadRequest(result.Error);
			}
			return Ok(result.MethodId);
		}

		[HttpPut("{shopId:guid}/paymentmethod/{methodId:guid}")]
		public async Task<ActionResult> UpdatePaymentMethod(UpdateMethodRequest request, Guid shopId, Guid methodId, CancellationToken cancellationToken)
		{
			var result = await _shopService.UpdatePaymentMethod(CurrentUser, shopId, methodId, request.Title, request.Metadata, cancellationToken);

			if (result == null)
			{
				return BadRequest();
			}
			if (!result.IsSuccess)
			{
				return BadRequest(result.Error);
			}
			return Ok(result.MethodId);
		}

		[HttpPut("{shopId:guid}/deliverymethod/{methodId:guid}")]
		public async Task<ActionResult> UpdateDeliveryMethod(UpdateMethodRequest request, Guid shopId, Guid methodId, CancellationToken cancellationToken)
		{
			var result = await _shopService.UpdateDeliveryMethod(CurrentUser, shopId, methodId, request.Title, request.Metadata, cancellationToken);

			if (result == null)
			{
				return BadRequest();
			}
			if (!result.IsSuccess)
			{
				return BadRequest(result.Error);
			}
			return Ok(result.MethodId);
		}

		[HttpPost("{shopId:guid}/colors")]
		public async Task<ActionResult> SaveColors(List<ColorSetting> colors, Guid shopId, CancellationToken cancellationToken)
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

			var colorsJson = JsonSerializer.Serialize(colors);
			var bytes = System.Text.Encoding.UTF8.GetBytes(colorsJson);

			using var stream = new MemoryStream(bytes);
			await _minioService.PutObject(shopId.ToString(), "colors.json", stream, "application/json", cancellationToken);

			return Ok();
		}

		[HttpGet("{shopId:guid}/colors")]
		public async Task<ActionResult<List<ColorSetting>>> GetColors(Guid shopId, CancellationToken cancellationToken)
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

			try
			{
				var minioObject = await _minioService.GetObject(shopId.ToString(), "colors.json", cancellationToken);
				using var reader = new StreamReader(minioObject);
				var content = await reader.ReadToEndAsync();
				var colors = JsonSerializer.Deserialize<List<ColorSetting>>(content);

				return Ok(colors);
			}
			catch
			{
				return NotFound("colors_not_found");
			}
		}
	}
}
