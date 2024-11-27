using Microsoft.AspNetCore.Mvc;
using ShareNote.Application.Services.Elasticsearchs;
using ShareNote.Domain.Entities;

namespace ShareNote.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ElasticsearchService _elasticsearchService;

        public SearchController(ILogger<SearchController> logger, ElasticsearchService elasticsearchService)
        {
            _logger = logger;
            _elasticsearchService = elasticsearchService;
        }

        // POST api/search/index
        [HttpPost("index")]
        public IActionResult IndexDocument([FromBody] WebPage page)
        {
            try
            {
                _elasticsearchService.IndexDocument(page);
                return Ok("Document indexed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error indexing document: {ex.Message}");
            }
        }

        // GET api/search
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string query)
        {
            try
            {
                var results = _elasticsearchService.Search(query);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error performing search: {ex.Message}");
            }
        }
    }
}
