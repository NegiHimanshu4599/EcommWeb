using AuthService.Application.DTOs;
using FluentValidation;

namespace AuthService.Application.Validators
{
    public sealed class LogoutValidator : AbstractValidator<LogoutDto>
    {
        public LogoutValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
