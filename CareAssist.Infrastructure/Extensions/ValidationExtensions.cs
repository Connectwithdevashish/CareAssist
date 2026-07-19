using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CareAssist.Api.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
