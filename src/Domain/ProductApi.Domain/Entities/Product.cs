namespace ProductApi.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTimeOffset ModifiedOn { get; set; }

    // Navigation property
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
