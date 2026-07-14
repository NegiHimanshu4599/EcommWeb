using AuthService.Application.DTOs;
using FluentValidation;

namespace AuthService.Application.Validators
{
    public sealed class UpdateUserProfileValidator:AbstractValidator<UpdateUserProfileDto>
    {
        public UpdateUserProfileValidator()
        {
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.PrimaryPhoneNumber).NotEmpty();
        }
    }
}