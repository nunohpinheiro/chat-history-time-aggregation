using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatHistory.ServiceApi.ApiConfiguration;

internal static class ApiResponsesExtensions
{
    internal static ValidationProblem ToValidationProblemDetails(this List<ValidationFailure> validationFailures)
    {
        var failureGroups = (validationFailures?
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            ?? Enumerable.Empty<IGrouping<string, string>>())
            .ToList();

        var problemErrors = new Dictionary<string, string[]>(failureGroups.Count);

        failureGroups.ForEach(failureGroup =>
            problemErrors.Add(
                failureGroup.Key,
                failureGroup.ToArray()));

        return TypedResults.ValidationProblem(problemErrors);
    }
}
