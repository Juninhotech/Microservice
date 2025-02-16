using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _context;
        private readonly HttpClient _httpClient;

        public ProductController(IHttpClientFactory httpClientFactory, ProductDbContext context)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct(int userId, string productName)
        {
            var user = await _httpClient.GetFromJsonAsync<UserDto>($"https://localhost:5293/api/users/{userId}");
            if (user == null) return BadRequest("Invalid user");

            var product = new Product { UserId = userId, ProductName = productName };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateProduct), new {product});
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
