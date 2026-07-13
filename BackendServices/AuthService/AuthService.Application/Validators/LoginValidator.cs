using AuthService.Application.DTOs;
using FluentValidation;

namespace AuthService.Application.Validators
{
    public sealed class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}