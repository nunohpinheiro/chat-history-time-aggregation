//using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace ChatHistory.ServiceApi.ApiConfiguration;

internal static class JsonSerializationExtensions
{
    internal static IServiceCollection AddCustomJsonSerialization(this IServiceCollection services)
        => services
        .Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .Configure<MvcJsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
}
