using DC_BOT.Commands.AnimeImages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC_BOT.Commands.AnimeImages
{
    internal static class AnimeImagesModule
    {
        internal static IServiceCollection AddAnimeImageCommands(this IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler, AnimeCommandHandler>();
            services.AddSingleton<ICommandHandler, AzurlaneCommandHandler>();
            services.AddSingleton<ICommandHandler, ChibiCommandHandler>();
            services.AddSingleton<ICommandHandler, ChristmasCommandHandler>();
            services.AddSingleton<ICommandHandler, DDLCCommandHandler>();
            services.AddSingleton<ICommandHandler, HalloweenCommandHandler>();
            services.AddSingleton<ICommandHandler, HoloCommandHandler>();
            services.AddSingleton<ICommandHandler, KitsuneCommandHandler>();
            services.AddSingleton<ICommandHandler, MaidCommandHandler>();
            services.AddSingleton<ICommandHandler, SenkoCommandHandler>();

            //services.AddSingleton<ICommandHandler, (Interaction)CommandHandler>();


            return services;
        }
    }
}
