namespace ERP.Domain.Entities;

public record class Category
{
    public int IdCategory { get; set; }
    public string CategoryName { get; set; } = null!;
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
