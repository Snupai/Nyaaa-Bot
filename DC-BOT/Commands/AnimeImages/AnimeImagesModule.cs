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
            services.AddSingleton<IAnimeImagesService, AnimeImagesService>();

            //services.AddSingleton<ICommandHandler, (Interaction)CommandHandler>();


            return services;
        }
    }
}
