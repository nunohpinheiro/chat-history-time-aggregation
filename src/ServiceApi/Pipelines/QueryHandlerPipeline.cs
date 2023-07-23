﻿using Ardalis.GuardClauses;
using FluentValidation;
using FluentValidation.Results;
using OneOf;
using Serilog;

namespace ChatHistory.ServiceApi.Pipelines;

internal abstract class QueryHandlerPipeline<Tin, Tout> where Tin : class
{
    private readonly Serilog.ILogger Logger = Log.Logger.ForContext<QueryHandlerPipeline<Tin, Tout>>();
    private readonly IValidator<Tin> QueryValidator;

    public QueryHandlerPipeline(IValidator<Tin> queryValidator)
    {
        QueryValidator = Guard.Against.Null(queryValidator);
    }

    protected abstract Task<Tout> Execute(Tin query);

    public async Task<OneOf<Tout, List<ValidationFailure>>> Handle(Tin query)
    {
        Logger.Information("Handling query request of type {QueryType} for object {Query}", typeof(Tin), query);

        var validationResult = await QueryValidator.ValidateAsync(query);
        if (!validationResult.IsValid)
            return validationResult.Errors;

        var result = await Execute(query);

        Logger.Information("Finished handling query request of type {QueryType}", typeof(Tin));
        return result;
    }
}
