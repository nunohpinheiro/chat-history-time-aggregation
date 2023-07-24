using Ardalis.GuardClauses;
using FluentValidation;
using FluentValidation.Results;
using OneOf;

namespace ChatHistory.ServiceApi.Pipelines;

internal abstract class QueryHandlerPipeline<Tin, Tout> where Tin : class
{
    protected readonly ILogger Logger;
    private readonly IValidator<Tin> QueryValidator;

    public QueryHandlerPipeline(ILogger logger, IValidator<Tin> queryValidator)
    {
        Logger = logger;
        QueryValidator = Guard.Against.Null(queryValidator);
    }

    protected abstract Task<Tout> Execute(Tin query, CancellationToken cancellationToken);

    public async Task<OneOf<Tout, List<ValidationFailure>>> Handle(Tin query, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Handling query request of type {QueryType} for object {Query}", typeof(Tin), query);

        var validationResult = await QueryValidator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            Logger.LogError("Errors validating {QueryType}. Details: {Errors}", typeof(Tin), validationResult.Errors);
            return validationResult.Errors;
        }

        var result = await Execute(query, cancellationToken);

        Logger.LogInformation("Finished handling query request of type {QueryType}", typeof(Tin));
        return result;
    }
}
