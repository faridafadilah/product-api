using AuthApi.Application.Interfaces;
using AuthApi.Models;
using AuthApi.Models.Common;
using AuthApi.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace AuthApi.Controllers;
/// Handles product CRUD operations.
/// Requires authenticated user.
[EnableRateLimiting("AuthLimiter")]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    private string? GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private string? GetUsername() =>
        User.FindFirst(ClaimTypes.Name)?.Value;

    /// Get all products.
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var result = await _service.GetAllAsync(search, category, page, limit);

        return Ok(new ApiResponse<object>(
            200,
            "Products retrieved successfully.",
            result.Data
        ));
    }


    /// Get product by ID.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(new ApiResponse<object>(404, result.Message));

        return Ok(new ApiResponse<Product>(
            200,
            "Product retrieved successfully.",
            result.Data
        ));
    }

    /// Create new product.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<object>(400, "Invalid request data."));

        var product = new Product
        {
            Title = dto.Title,
            Price = dto.Price,
            Description = dto.Description,
            Category = dto.Category,
            Images = dto.Images,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = GetUsername(),
            CreatedById = GetUserId(),
            UpdatedBy = GetUsername(),
            UpdatedById = GetUserId()
        };

        var result = await _service.CreateAsync(product);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            new ApiResponse<Product>(201, result.Message, result.Data)
        );
    }

    /// Update existing product.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductRequestDto dto)
    {
        var existing = await _service.GetByIdAsync(id);

        if (!existing.Success || existing.Data == null)
            return NotFound(new ApiResponse<object>(404, "Product not found."));

        var product = existing.Data;
        product.Title = dto.Title;
        product.Price = dto.Price;
        product.Description = dto.Description;
        product.Category = dto.Category;
        product.Images = dto.Images;
        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = GetUsername();
        product.UpdatedById = GetUserId();

        var result = await _service.UpdateAsync(product);

        return Ok(new ApiResponse<Product>(
            200,
            result.Message,
            result.Data
        ));
    }

    /// Delete product by ID.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.Success)
            return NotFound(new ApiResponse<object>(404, result.Message));

        return Ok(new ApiResponse<object>(200, result.Message));
    }
}