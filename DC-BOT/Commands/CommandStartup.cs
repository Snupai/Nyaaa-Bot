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
            var globalSlashCommands = new List<SlashCommandProperties>();
            var guildSlashCommands = new List<SlashCommandProperties>();


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

            await this.MigrateCommands(globalSlashCommands, guildSlashCommands);
        }

        private async Task MigrateCommands(List<SlashCommandProperties> desiredGlobalCommands, List<SlashCommandProperties> desiredGuildCommands) 
        {
            Console.WriteLine("Do you want to migrate commands? If so type 'yes'");
            var response = Console.ReadLine();

            if (response != "yes") return;

            Console.WriteLine("[Command Migration] Starting migration");
            var clientAppInfo = await client.GetApplicationInfoAsync();

            var globalCommands = await client.GetGlobalApplicationCommandsAsync();
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

            foreach (var desiredGlobalCommand in desiredGlobalCommands)
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

            client.Ready += async () =>
            {
                var guildId = Environment.GetEnvironmentVariable("guildId");
                var guild = this.client.GetGuild(UInt64.Parse(guildId));

                var guildCommands = await guild.GetApplicationCommandsAsync();

                foreach (SocketApplicationCommand guildCommand in guildCommands)
                {
                    if (guildCommand.Type != ApplicationCommandType.Slash)
                    {
                        Console.WriteLine($"[Command Migration] Skipping guild command '{guildCommand.Name}' for guild {guildId} because it isn't a slash command");
                    }

                    if (guildCommand.Id != clientAppInfo.Id)
                    {
                        Console.WriteLine($"[Command Migration] Skipping guild command '{guildCommand.Name}' for guild {guildId} because it belongs to a different application");
                        continue;
                    }

                    Console.WriteLine($"[Command Migration] Deleting command '{guildCommand.Name}' for guild {guildId}");

                    await guildCommand.DeleteAsync();
                }

                foreach (var desiredGuildCommand in desiredGuildCommands)
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
            };
        }
    }
}
