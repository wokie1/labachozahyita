using System;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProductsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredProducts()
    {
        var expired = await _context.Products
            .Where(p => p.ExpiryDate < DateTime.Now)
            .ToListAsync();
        return Ok(expired);
    }
}