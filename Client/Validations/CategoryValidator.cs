using Client.Components.Pages.Administrator;
using FluentValidation;
using SharedLibrary;

namespace Client.Validations
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<Category>.CreateWithOptions((Category)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public CategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category Name is required.");            
        }
    }
}
