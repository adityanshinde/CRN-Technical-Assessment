namespace ProductApi.Application.DTOs;

public class ItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateItemDto
{
    public int Quantity { get; set; }
}
