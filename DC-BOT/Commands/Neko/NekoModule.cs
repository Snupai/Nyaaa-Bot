using Microsoft.Extensions.DependencyInjection;

namespace DC_BOT.Commands.Neko
{
    internal static class NekoModule
    {
        internal static IServiceCollection AddNekoCommands(this IServiceCollection services) {
            services.AddSingleton<INekoService, NekoService>();

            services.AddSingleton<ICommandHandler,  NekoCommandHandler>();
            services.AddSingleton<ICommandHandler, NekoBoyCommandHandler>();
            services.AddSingleton<ICommandHandler, NekoGifCommandHandler>();
            services.AddSingleton<ICommandHandler, NekoParaCommandHandler>();

            return services;
        }
    }
}
