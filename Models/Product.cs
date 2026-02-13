namespace AuthApi.Models;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public List<string> Images { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedById { get; set; }
}
