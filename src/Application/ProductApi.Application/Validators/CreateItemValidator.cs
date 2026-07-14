using FluentValidation;
using ProductApi.Application.DTOs;

namespace ProductApi.Application.Validators;

public class CreateItemValidator : AbstractValidator<CreateItemDto>
{
    public CreateItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}

public class UpdateItemValidator : AbstractValidator<UpdateItemDto>
{
    public UpdateItemValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
