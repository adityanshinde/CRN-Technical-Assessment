using FluentValidation;
using ProductApi.Application.DTOs;

namespace ProductApi.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.ModifiedBy)
            .NotEmpty()
            .MaximumLength(100);
    }
}
