using DC_BOT.Commands.Interactions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC_BOT.Commands.Interactions
{
    internal static class InteractionsModule
    {
        internal static IServiceCollection AddInteractionCommands(this IServiceCollection services)
        {
            services.AddSingleton<IInteractionsService, InteractionsService>();

            //services.AddSingleton<ICommandHandler, (Interaction)CommandHandler>();
            

            return services;
        }
    }
}
