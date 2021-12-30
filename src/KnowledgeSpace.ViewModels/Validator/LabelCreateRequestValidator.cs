using FluentValidation;
using KnowledgeSpace.ViewModels.Other;
using KnowledgeSpace.ViewModels.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Validator
{
    public class LabelCreateRequestValidator : AbstractValidator<LabelCreateRequest>
    {
        public LabelCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(string.Format(Messages.Required, "Tên"));
        }
    }
}