using FluentValidation;
using KnowledgeSpace.ViewModels.Other;
using KnowledgeSpace.ViewModels.Requests;

namespace KnowledgeSpace.ViewModels.Validator
{
    public class VoteCreateRequestValidator : AbstractValidator<VoteCreateRequest>
    {
        public VoteCreateRequestValidator()
        {
            RuleFor(x => x.KnowledgeBaseId)
                .GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, "Mã bài đăng"));
        }
    }
}