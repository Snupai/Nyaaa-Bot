using DC_BOT.Commands.nsfwAnimeImages;
using Discord.Net;
using Microsoft.Extensions.DependencyInjection;

namespace DC_BOT.Commands.nsfwAnimeImages
{
    internal static class nsfwAnimeImagesModule
    {
        internal static IServiceCollection AddnsfwAnimeImageCommands(this IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler, nsfwAnalCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwAnusCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwAzurlaneCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwBdsmCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwBlowjobCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwBoobsCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwCosplayCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwCumCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwFeetCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwFemdomCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwFutaCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwGasmCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwHoloCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwKitsuneCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwLewdCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwNekoCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwNekoparaCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwPantyhoseCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwPeeingCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwPetplayCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwPussyCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwSlimesCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwSoloCommandHandler>();
            //services.AddSingleton<ICommandHandler, nsfwSwimsuitCommandHandler>(); api bad request
            services.AddSingleton<ICommandHandler, nsfwTentacleCommandHandler>();
            //services.AddSingleton<ICommandHandler, nsfwThighsCommandHandler>(); api bad request
            services.AddSingleton<ICommandHandler, nsfwTrapCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwYaoiCommandHandler>();
            services.AddSingleton<ICommandHandler, nsfwYuriCommandHandler>();

            //services.AddSingleton<ICommandHandler, (Interaction)CommandHandler>();

            return services;
        }
    }
}
