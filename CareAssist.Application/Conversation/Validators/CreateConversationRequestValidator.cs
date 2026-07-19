using CareAssist.Contracts.Conversations;
using FluentValidation;

namespace CareAssist.Application.Conversation.Validators;

public sealed class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequest>
{
    public CreateConversationRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.");
    }
}
