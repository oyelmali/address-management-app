using FluentValidation;
using AddressApp.Application.DTOs;

namespace AddressApp.Application.Validators;

public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
{
    public CreateAddressDtoValidator()
    {
        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region is required")
            .MaximumLength(100);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100);

        RuleFor(x => x.BranchNumber)
            .NotEmpty().WithMessage("Branch number is required")
            .MaximumLength(50);

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(300);

        RuleFor(x => x.Phone)
            .MaximumLength(20);
    }
}

public class UpdateAddressDtoValidator : AbstractValidator<UpdateAddressDto>
{
    public UpdateAddressDtoValidator()
    {
        RuleFor(x => x.Region)
            .NotEmpty().WithMessage("Region is required")
            .MaximumLength(100);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100);

        RuleFor(x => x.BranchNumber)
            .NotEmpty().WithMessage("Branch number is required")
            .MaximumLength(50);

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(300);

        RuleFor(x => x.Phone)
            .MaximumLength(20);
    }
}