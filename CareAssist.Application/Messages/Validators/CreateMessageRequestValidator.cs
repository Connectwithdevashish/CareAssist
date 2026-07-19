using CareAssist.Contracts.Messages;
using FluentValidation;

namespace CareAssist.Application.Messages.Validators;

public sealed class CreateMessageRequestValidator : AbstractValidator<CreateMessageRequest>
{
    public CreateMessageRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content cannot be empty.")
            .MaximumLength(4000).WithMessage("Message content cannot exceed 4000 characters.");
    }
}
