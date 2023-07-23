using Ardalis.GuardClauses;
using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using Serilog;

namespace ChatHistory.ServiceApi.Pipelines;

internal abstract class CommandHandlerPipeline<Tin> where Tin : class
{
    private readonly Serilog.ILogger Logger = Log.Logger.ForContext<CommandHandlerPipeline<Tin>>();
    private readonly IValidator<Tin> CommandValidator;

    public CommandHandlerPipeline(IValidator<Tin> commandValidator)
    {
        CommandValidator = Guard.Against.Null(commandValidator);
    }

    protected abstract Task Execute(Tin command);

    public async Task<OneOf<Success, List<ValidationFailure>>> Handle(Tin command)
    {
        Logger.Information("Handling command request of type {CommandType} for object {Command}", typeof(Tin), command);

        var validationResult = await CommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return validationResult.Errors;

        await Execute(command);

        Logger.Information("Finished handling command request of type {CommandType}", typeof(Tin));
        return new Success();
    }
}
