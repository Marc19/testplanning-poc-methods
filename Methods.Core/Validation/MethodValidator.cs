using System;
using FluentValidation;
using Methods.Core.Entities;

namespace Methods.Core.Validation
{
    public class MethodValidator : AbstractValidator<Method>
    {
        public MethodValidator()
        {
            RuleFor(exp => exp.Creator).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(exp => exp.Name).NotNull().NotEmpty().MinimumLength(3);
            RuleFor(exp => exp.ApplicationRate).GreaterThan(0).LessThanOrEqualTo(1);
            RuleFor(exp => exp.CreationDate).NotNull().NotEmpty().GreaterThanOrEqualTo(DateTime.Today);
        }
    }
}
