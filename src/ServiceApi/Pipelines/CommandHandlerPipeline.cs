using Ardalis.GuardClauses;
using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;

namespace ChatHistory.ServiceApi.Pipelines;

internal abstract class CommandHandlerPipeline<Tin> where Tin : class
{
    protected readonly ILogger Logger;
    private readonly IValidator<Tin> CommandValidator;

    public CommandHandlerPipeline(ILogger logger, IValidator<Tin> commandValidator)
    {
        Logger = logger;
        CommandValidator = Guard.Against.Null(commandValidator);
    }

    protected abstract Task Execute(Tin command, CancellationToken cancellationToken);

    public async Task<OneOf<Success, List<ValidationFailure>>> Handle(Tin command, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Handling command request of type {CommandType} for object {Command}", typeof(Tin), command);

        var validationResult = await CommandValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            Logger.LogError("Errors validating {CommandType}. Details: {Errors}", typeof(Tin), validationResult.Errors);
            return validationResult.Errors;
        }

        await Execute(command, cancellationToken);

        Logger.LogInformation("Finished handling command request of type {CommandType}", typeof(Tin));
        return new Success();
    }
}
