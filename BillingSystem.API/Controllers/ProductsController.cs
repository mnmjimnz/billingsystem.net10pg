using BillingSystem.Domain.Entities;
using BillingSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BillingSystem.API.Extensions;

namespace BillingSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? branchId = null)
    {
        if (!User.IsAdmin())
        {
            branchId = User.GetBranchId();
        }
        return Ok(await _productRepository.GetPagedAsync(search, page, pageSize, branchId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("{id}/stock")]
    public async Task<IActionResult> GetStockByBranch(int id)
    {
        var stock = await _productRepository.GetStockByBranchAsync(id);
        return Ok(stock);
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode)
    {
        var product = await _productRepository.GetByBarcodeAsync(barcode);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        var id = await _productRepository.AddAsync(product);
        product.Id = id;
        return CreatedAtAction(nameof(GetById), new { id = id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id) return BadRequest();
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing == null) return NotFound();
        product.ImageUrl = existing.ImageUrl;
        await _productRepository.UpdateAsync(product);
        return NoContent();
    }

    [HttpPost("{id}/image")]
    [Authorize]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
        if (string.IsNullOrEmpty(cloudinaryUrl))
        {
            return StatusCode(500, new { message = "Cloudinary environment variable is not configured on the server." });
        }

        if (cloudinaryUrl.StartsWith("CLOUDINARY_URL="))
        {
            cloudinaryUrl = cloudinaryUrl.Substring("CLOUDINARY_URL=".Length);
        }

        var cloudinary = new CloudinaryDotNet.Cloudinary(cloudinaryUrl);
        cloudinary.Api.Secure = true;

        using var stream = file.OpenReadStream();
        var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
        {
            File = new CloudinaryDotNet.FileDescription(file.FileName, stream),
            PublicId = $"product_{id}_{Guid.NewGuid()}",
            Folder = "products",
            Overwrite = true
        };

        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            return StatusCode(500, new { message = uploadResult.Error.Message });
        }

        var fileUrl = uploadResult.SecureUrl.ToString();
        
        product.ImageUrl = fileUrl;
        await _productRepository.UpdateAsync(product);

        return Ok(new { ImageUrl = fileUrl });
    }
}
