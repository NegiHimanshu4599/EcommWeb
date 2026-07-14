using AuthService.Application.DTOs;
using FluentValidation;

namespace AuthService.Application.Validators
{
    public sealed class UpdateAddressValidator:AbstractValidator<UpdateAddressDto>
    {
        public UpdateAddressValidator()
        {
            RuleFor(x => x.AddressLine1).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.State).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
            RuleFor(x => x.PostalCode).NotEmpty();
            RuleFor(x => x.ContactPhoneNumber).NotEmpty();
        }
    }
}
