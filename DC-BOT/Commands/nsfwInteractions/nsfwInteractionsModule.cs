using Microsoft.Extensions.DependencyInjection;

namespace DC_BOT.Commands.nsfwInteractions
{
    internal static class nsfwInteractionsModule
    {
        internal static IServiceCollection AddnsfwInteractionCommands(this IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler, nsfwAnalCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwAssCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwBdsmCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwBlowjobCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwBoobjobCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwBoobsCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwCumCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwFeetCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwFutaCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwHandjobCommandHandler>();
            //services.AddSingleton<ICommandHandler, nsfwKitsuneCommandHandler>();      nyot yet implemented
            //services.AddSingleton<ICommandHandler, nsfwFemdomCommandHandler>();       nyot yet implemented
            services.AddSingleton<ICommandHandler, nsfwKuniCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwNekoCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwPussyCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwSoloCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwSpankCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwTentacleCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwToysCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwYuriCommandHandler>();


            //services.AddSingleton<ICommandHandler, (Interaction)CommandHandler>();


            return services;
        }
    }
}
