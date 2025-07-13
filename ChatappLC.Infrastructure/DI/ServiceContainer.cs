namespace ChatappLC.Infrastructure.DI;

public static class ServiceContainer
{
    public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
        services.AddScoped<IChatRoomService, ChatRoomService>();
        return services;
    }
}
