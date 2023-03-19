using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Discord.Net;
using Newtonsoft.Json;

namespace DC_BOT.Commands
{
    internal class CommandStartup
    {
        private readonly DiscordSocketClient client;
        private readonly IEnumerable<ICommandHandler> commandHandlers;

        public CommandStartup(DiscordSocketClient client, IHost host)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.commandHandlers = host.Services.GetServices<ICommandHandler>();

            if (!commandHandlers.Any()) 
            {
                throw new ArgumentException("No registered command handlers in host");
            }
        }

        internal async Task Start() 
        {
            foreach (var commandHandler in this.commandHandlers)
            {
                var commandProperties = commandHandler.Initialize();

                if (!commandProperties.Name.IsSpecified) 
                {
                    throw new ArgumentNullException(nameof(commandProperties.Name));
                }

                this.client.SlashCommandExecuted += async (SocketSlashCommand command) =>
                {
                    if (command.Data.Name == commandProperties.Name.Value)
                    {
                        await commandHandler.HandleAsync(command);
                    }
                };

                try
                {
                    await client.CreateGlobalApplicationCommandAsync(commandProperties);
                }
                catch (HttpException exception)
                {
                    // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                    var json = JsonConvert.SerializeObject(exception.Message, Formatting.Indented);

                    // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                    Console.WriteLine(json);
                }
            }
        }
    }
}
