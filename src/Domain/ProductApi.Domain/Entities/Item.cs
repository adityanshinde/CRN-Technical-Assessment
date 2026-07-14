namespace ProductApi.Domain.Entities;

public class Item
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    // Navigation property
    public Product Product { get; set; } = null!;
}
