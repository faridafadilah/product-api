namespace AuthApi.Models.DTOs;
public class ProductRequestDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
     public List<string> Images { get; set; } = new();
}

