using FluentValidation;
using KnowledgeSpace.ViewModels.Other;
using KnowledgeSpace.ViewModels.Requests;
namespace KnowledgeSpace.ViewModels.Validator
{
    public class CategoryCreateRequestValidator : AbstractValidator<CategoryCreateRequest>
    {
        public CategoryCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(string.Format(Messages.Required, "Tên"));

            RuleFor(x => x.SeoAlias).NotEmpty().WithMessage(string.Format(Messages.Required, "Seo alias"));
        }
    }
}