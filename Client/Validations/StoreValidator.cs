using Client.Components.Pages.Administrator;
using FluentValidation;
using SharedLibrary;

namespace Client.Validations
{
    public class StoreValidator : AbstractValidator<StoreApplications>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<StoreApplications>.CreateWithOptions((StoreApplications)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public StoreValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Store Name is required.");

            RuleFor(x => x.TIN)
                .NotEmpty().WithMessage("T.I.N No. is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email Address is required.")
                .EmailAddress().WithMessage("Invalid Email Address format.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone Number is required.")
                .Matches(@"^\d{11}$").WithMessage("Phone Number must be 11 digits.");

            //RuleFor(x => x.AddressLine1)
            //    .NotEmpty().WithMessage("Address Line 1 is required.");

            //RuleFor(x => x.CityTown)
            //    .NotEmpty().WithMessage("City/Town is required.");

            //RuleFor(x => x.Province)
            //    .NotEmpty().WithMessage("Province is required.");

            //RuleFor(x => x.PostalCode)
            //    .NotEmpty().WithMessage("Postal Code is required.")
            //    .Matches(@"^\d{4}$").WithMessage("Postal Code must be 4 digits.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Store Description is required.");
        }
    }
}
