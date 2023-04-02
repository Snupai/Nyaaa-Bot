using Microsoft.Extensions.DependencyInjection;

namespace DC_BOT.Commands.Interactions
{
    internal static class InteractionsModule
    {
        internal static IServiceCollection AddInteractionCommands(this IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler, BakaCommandHandler>();
            services.AddSingleton<ICommandHandler, BiteCommandHandler>();
            services.AddSingleton<ICommandHandler, BlushCommandHandler>();
            services.AddSingleton<ICommandHandler, CryCommandHandler>();
            services.AddSingleton<ICommandHandler, DanceCommandHandler>();
            services.AddSingleton<ICommandHandler, FeedCommandHandler>();
            services.AddSingleton<ICommandHandler, FluffCommandHandler>();
            services.AddSingleton<ICommandHandler, GrabCheeksCommandHandler>();
            services.AddSingleton<ICommandHandler, HandHoldCommandHandler>();
            services.AddSingleton<ICommandHandler, HighfiveCommandHandler>();
            services.AddSingleton<ICommandHandler, HugCommandHandler>();
            services.AddSingleton<ICommandHandler, KissCommandHandler>();
            services.AddSingleton<ICommandHandler, LickCommandHandler>();
            services.AddSingleton<ICommandHandler, PatCommandHandler>();
            services.AddSingleton<ICommandHandler, PokeCommandHandler>();
            services.AddSingleton<ICommandHandler, PunchCommandHandler>();
            services.AddSingleton<ICommandHandler, SlapCommandHandler>();
            services.AddSingleton<ICommandHandler, ShrugCommandHandler>();
            services.AddSingleton<ICommandHandler, SmugCommandHandler>();
            services.AddSingleton<ICommandHandler, StareCommandHandler>();
            services.AddSingleton<ICommandHandler, WagCommandHandler>();
            services.AddSingleton<ICommandHandler, TickleCommandHandler>();
            services.AddSingleton<ICommandHandler, WaveCommandHandler>();
            services.AddSingleton<ICommandHandler, WinkCommandHandler>();

            //services.AddSingleton<ICommandHandler, (Interaction)CommandHandler>();


            return services;
        }
    }
}
