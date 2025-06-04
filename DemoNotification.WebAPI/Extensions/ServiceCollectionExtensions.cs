using DemoNotification.WebAPI.Validators;
using FluentValidation;

namespace DemoNotification.WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModelsValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<NotificationRequestValidator>();

        return services;
    }
}
