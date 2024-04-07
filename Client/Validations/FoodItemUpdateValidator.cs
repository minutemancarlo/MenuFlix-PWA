using Client.Components.Pages.Administrator;
using FluentValidation;
using SharedLibrary;

namespace Client.Validations
{
    public class FoodItemUpdateValidator : AbstractValidator<FoodItem>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<FoodItem>.CreateWithOptions((FoodItem)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public FoodItemUpdateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Food Item Name is required.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Food Item Description is required.");            
            RuleFor(x => x.Logo)
                .NotEmpty().WithMessage("Food Item Image is required.");
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Food Item Category is required.");
            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Food Item price is required.")
                .GreaterThan(0).WithMessage("Food Item price must be greater than zero.");
            
                
        }


    }
}
