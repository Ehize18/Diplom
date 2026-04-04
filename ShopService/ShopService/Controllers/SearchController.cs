using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopService.Database.Interfaces;

namespace ShopService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SearchController : ControllerBase
	{
		private readonly ISearchRepository _searchRepository;

		public SearchController(ISearchRepository searchRepository)
		{
			_searchRepository = searchRepository;
		}

		[HttpGet]
		public async Task<ActionResult> Search(string query, CancellationToken cancellationToken = default)
		{
			var results = await _searchRepository.SearchAsync(query, cancellationToken: cancellationToken);

			return Ok(results);
		}
	}
}
