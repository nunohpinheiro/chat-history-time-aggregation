using ChatHistory.Domain.ChatRecords;
using ChatHistory.Domain.ValueObjects;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ChatHistory.ServiceApi.ChatRecords;

public record CreateChatRecordCommand(
    string EventType,
    string Timestamp,
    string User,
    string CommentText,
    string HighFivedPerson);

public class CreateChatRecordCommandValidator : AbstractValidator<CreateChatRecordCommand>
{
    private readonly Regex UserFormat = new(Username.UserFormatRegex, RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public CreateChatRecordCommandValidator()
    {
        RuleFor(c => c).NotEmpty();

        RuleFor(c => c.EventType)
            .NotEmpty()
            .Must(et => et.TryGetEventType(out _))
            .WithMessage(command => $"Value '{command.EventType}' is not valid for property '{{PropertyName}}'");

        RuleFor(c => c.Timestamp)
            .NotEmpty()
            .Must(t => t.TryGetUtcDateTime(out _))
            .WithMessage($"'{{PropertyName}}' must have a valid format: '{UtcDateTime.ValidFormat}'");

        RuleFor(c => c.User)
            .NotEmpty()
            .Matches(UserFormat)
            .WithMessage($"'{{PropertyName}}' must have a valid format: '{Username.UserFormatRegex}'");

        RuleFor(c => c.CommentText)
            .Must((command, _) => CheckCommentTextIsValid(command))
            .WithMessage(command => SetMessagePropertyMustBeFulfilled(command.EventType));

        RuleFor(c => c.HighFivedPerson)
            .Must((command, _) => CheckHighFivedPersonIsValid(command))
            .WithMessage(command => SetMessagePropertyMustBeFulfilled(command.EventType))
            .Matches(UserFormat)
            .WithMessage($"'{{PropertyName}}' must have a valid format: '{Username.UserFormatRegex}'");
    }

    private static bool CheckCommentTextIsValid(CreateChatRecordCommand command)
        => CheckEventDetailIsValid(command.EventType, command.CommentText, EventType.Comment);

    private static bool CheckHighFivedPersonIsValid(CreateChatRecordCommand command)
        => CheckEventDetailIsValid(command.EventType, command.HighFivedPerson, EventType.HighFiveOtherUser);

    private static bool CheckEventDetailIsValid(string actualEventType, string eventDetail, EventType referenceEventType)
        => (!actualEventType.TryGetEventType(out var eventType))
        || (eventType != referenceEventType)
        || !string.IsNullOrWhiteSpace(eventDetail);

    private static string SetMessagePropertyMustBeFulfilled(string eventType)
        => $"'{{PropertyName}}' must be fulfilled for events of type {eventType}";
}
