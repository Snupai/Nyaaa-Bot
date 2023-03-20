using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Discord.Net;
using Newtonsoft.Json;
using Discord;

namespace DC_BOT.Commands
{
    internal class CommandStartup
    {
        private const bool ShouldDelete = false;

        private readonly DiscordSocketClient client;
        private readonly IEnumerable<ICommandHandler> commandHandlers;

        private List<SlashCommandProperties> globalSlashCommands = new List<SlashCommandProperties>();
        private List<SlashCommandProperties> guildSlashCommands = new List<SlashCommandProperties>();

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

                if (commandHandler.IsGuildCommand)
                {
                    guildSlashCommands.Add(commandProperties);
                }
                else {
                    globalSlashCommands.Add(commandProperties);
                }
            }
        }

        internal async Task MigrateGuildCommands() {
            var guildId = Environment.GetEnvironmentVariable("guildId");
            Console.WriteLine($"[Command Migration] Starting guild command migration for '{guildId}'");

            var clientAppInfo = await client.GetApplicationInfoAsync();

            var guild = this.client.GetGuild(UInt64.Parse(guildId));

            var guildCommands = await guild.GetApplicationCommandsAsync();

            if (ShouldDelete)
            {
                foreach (SocketApplicationCommand guildCommand in guildCommands)
                {
                    if (guildCommand.Type != ApplicationCommandType.Slash)
                    {
                        Console.WriteLine($"[Command Migration] Skipping guild command '{guildCommand.Name}' for guild {guildId} because it isn't a slash command");
                    }

                    if (guildCommand.ApplicationId != clientAppInfo.Id)
                    {
                        Console.WriteLine($"[Command Migration] Skipping guild command '{guildCommand.Name}' for guild {guildId} because it belongs to a different application");
                        continue;
                    }

                    Console.WriteLine($"[Command Migration] Deleting command '{guildCommand.Name}' for guild {guildId}");

                    await guildCommand.DeleteAsync();
                }
            }            

            foreach (var desiredGuildCommand in guildSlashCommands)
            {
                try
                {
                    Console.WriteLine($"[Command Migration] Creating command '{desiredGuildCommand.Name}' for guild {guildId}");
                    await guild.CreateApplicationCommandAsync(desiredGuildCommand);
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

        internal async Task MigrateCommands() 
        {
            Console.WriteLine("[Command Migration] Starting migration");

            IReadOnlyCollection<SocketApplicationCommand> globalCommands;

            try
            {
                globalCommands = await client.GetGlobalApplicationCommandsAsync();
            }
            catch {
                return;
            }
            if (globalCommands == null || !globalCommands.Any()) return;

            var clientAppInfo = await client.GetApplicationInfoAsync();
            if (ShouldDelete) 
            {
                foreach (SocketApplicationCommand globalCommand in globalCommands)
                {
                    if (globalCommand.Type != ApplicationCommandType.Slash)
                    {
                        Console.WriteLine($"[Command Migration] Skipping command '{globalCommand.Name}' because it isn't a slash command");
                    }

                    if (globalCommand.ApplicationId != clientAppInfo.Id) {
                        Console.WriteLine($"[Command Migration] Skipping command '{globalCommand.Name}' because it belongs to a different application");
                        continue;
                    }

                    Console.WriteLine($"[Command Migration] Deleting command '{globalCommand.Name}'");

                    await globalCommand.DeleteAsync();                
                }
            }

            foreach (var desiredGlobalCommand in globalSlashCommands)
            {
                try
                {
                    Console.WriteLine($"[Command Migration] Creating command '{desiredGlobalCommand.Name}'");
                    await client.CreateGlobalApplicationCommandAsync(desiredGlobalCommand);
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
