using CareAssist.Application.Authentication;
using CareAssist.Application.Conversation;
using CareAssist.Application.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace CareAssist.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidation();

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IMessageService, MessageService>();

        return services;
    }
}
