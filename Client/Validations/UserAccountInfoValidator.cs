using FluentValidation;
using SharedLibrary;

namespace Client.Validations
{
    public class UserAccountInfoValidator : AbstractValidator<UserAdditionalDetails>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserAdditionalDetails>.CreateWithOptions((UserAdditionalDetails)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public UserAccountInfoValidator()
        {
            RuleFor(user => user.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(user => user.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(user => user.PhoneNumber).NotEmpty().WithMessage("Phone number is required.")
                                             .Matches(@"^\d{11}$").WithMessage("Phone number must be 11 digits.");
            RuleFor(user => user.AddressLine1).NotEmpty().WithMessage("Address line 1 is required.");
            RuleFor(user => user.CityTown).NotEmpty().WithMessage("City/Town is required.");
            RuleFor(user => user.Province).NotEmpty().WithMessage("Province is required.");
            RuleFor(user => user.PostalCode).NotEmpty().WithMessage("Postal code is required.");
        }
    }
}
