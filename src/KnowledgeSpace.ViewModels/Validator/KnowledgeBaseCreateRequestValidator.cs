using FluentValidation;
using KnowledgeSpace.ViewModels.Other;
using KnowledgeSpace.ViewModels.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Validator

{
    public class KnowledgeBaseCreateRequestValidator : AbstractValidator<KnowledgeBaseCreateRequest>
    {
        public KnowledgeBaseCreateRequestValidator()
        {
            RuleFor(x => x.CategoryId).GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, "Danh mục"));

            RuleFor(x => x.Title).NotEmpty().WithMessage(string.Format(Messages.Required, "Tiêu đề"));

            RuleFor(x => x.Problem).NotEmpty().WithMessage(string.Format(Messages.Required, "Vấn đề"));

            RuleFor(x => x.Note).NotEmpty().WithMessage(string.Format(Messages.Required, "Giải pháp"));
        }
    }
}