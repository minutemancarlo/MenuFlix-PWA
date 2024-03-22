using FluentValidation;
using SharedLibrary;

namespace Client.Validations
{
   
    public class FoodItemDiscountValidator : AbstractValidator<FoodItemDiscount>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<FoodItemDiscount>.CreateWithOptions((FoodItemDiscount)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public FoodItemDiscountValidator()
        {
            RuleFor(x => x.FoodItemId)
                .NotEmpty().WithMessage("Food Item is required.");
            RuleFor(x => x.DiscountName)
                .NotEmpty().WithMessage("Discount Name is required.");            
            RuleFor(x => x.DiscountAmount)
                .NotEmpty().WithMessage("Food Item price is required.")
                .GreaterThan(0).WithMessage("Food Item price must be greater than zero.");
        }
    }
}
