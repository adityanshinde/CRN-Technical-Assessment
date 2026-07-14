namespace ProductApi.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTimeOffset ModifiedOn { get; set; }
    public List<ItemDto> Items { get; set; } = new();
}

public class CreateProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}

public class UpdateProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
}
