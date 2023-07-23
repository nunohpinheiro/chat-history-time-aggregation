using ChatHistory.Domain.ChatRecords;
using ChatHistory.Domain.ValueObjects;
using FluentValidation;

namespace ChatHistory.ServiceApi.ChatRecords;

public record ReadChatRecordsQuery(
    Granularity Granularity,
    int PageNumber,
    int PageSize,
    string StartDateTime,
    string EndDateTime);

public class ReadChatRecordsQueryValidator : AbstractValidator<ReadChatRecordsQuery>
{
    private const string MsgMustBePositiveInt = "'{PropertyName}' must be a positive integer";
    private const string MsgMustBeValidDateTime = $"'{{PropertyName}}' must have a valid format: '{UtcDateTime.ValidFormat}'";

    public ReadChatRecordsQueryValidator()
    {
        RuleFor(q => q).NotEmpty();

        RuleFor(q => q.PageNumber)
            .GreaterThan(0)
            .WithMessage(MsgMustBePositiveInt);

        RuleFor(q => q.PageSize)
            .GreaterThan(0)
            .WithMessage(MsgMustBePositiveInt);

        RuleFor(q => q.StartDateTime)
            .NotEmpty()
            .Must(t => t.TryGetUtcDateTime(out _))
            .WithMessage(MsgMustBeValidDateTime);

        RuleFor(q => q.EndDateTime)
            .NotEmpty()
            .Must(t => t.TryGetUtcDateTime(out _))
            .WithMessage(MsgMustBeValidDateTime);
    }
}
