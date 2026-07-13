using AuthService.Application.DTOs;
using FluentValidation;

namespace AuthService.Application.Validators
{
    public sealed class GoogleLoginValidator : AbstractValidator<GoogleLoginDto>
    {
        public GoogleLoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }
    }
}
