using DC_BOT.Commands.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC_BOT.Commands.Utility
{
    internal static class UtilityModule
    {
        internal static IServiceCollection AddUtilityCommands(this IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler, BanCommandHandler>();
            services.AddSingleton<ICommandHandler, UnbanCommandHandler>();
            services.AddSingleton<ICommandHandler, KickCommandHandler>();
            services.AddSingleton<ICommandHandler,MuteCommandHandler>();
            services.AddSingleton<ICommandHandler,UnmuteCommandHandler>();
            //services.AddSingleton<ICommandHandler, (Interaction)CommandHandler>();
            return services;
        }
    }
}
