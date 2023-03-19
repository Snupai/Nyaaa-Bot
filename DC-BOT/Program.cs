using DC_BOT.Commands;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace DNet_V3_Tutorial
{
    public class program
    {
        private DiscordSocketClient _client;

        // Program entry point
        public static Task Main(string[] args) => new program().MainAsync();

        public async Task MainAsync()
        {
            var config = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "&")
            // this will be used more later on
            .SetBasePath(AppContext.BaseDirectory)
            // I chose using YML files for my config data as I am familiar with them
            .AddYamlFile("config.yml")
            .Build();
            Environment.SetEnvironmentVariable("apiKey", config["tokens:fluxpoint-api"]);
            Environment.SetEnvironmentVariable("guildId", config["testGuild"]);
            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    services
                    // Add the configuration to the registered services
                    .AddSingleton(config)
                    // Add the DiscordSocketClient, along with specifying the GatewayIntents and user caching
                    .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                    {
                        GatewayIntents = Discord.GatewayIntents.AllUnprivileged,
                        LogGatewayIntentWarnings = false,
                        AlwaysDownloadUsers = true,
                        LogLevel = LogSeverity.Debug
                    }))
                    // Adding console logging
                    .AddTransient<ConsoleLogger>()
                    .AddTransient<ILogger, ConsoleLogger>()
                    // Used for slash commands and their registration with Discord
                    .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                    // Required to subscribe to the various client events used in conjunction with Interactions
                    .AddSingleton<InteractionHandler>()
                    // Adding the prefix Command Service
                    .AddSingleton(x => new CommandService(new CommandServiceConfig
                    {
                        LogLevel = LogSeverity.Debug,
                        DefaultRunMode = Discord.Commands.RunMode.Async
                    }))
                    // Add new command handlers here
                    .AddSingleton<ICommandHandler, BakaCommandHandler>()
                )
                .Build();

            await RunAsync(host);
        }

        public async Task RunAsync(IHost host)
        {

            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var commands = provider.GetRequiredService<InteractionService>();
            _client = provider.GetRequiredService<DiscordSocketClient>();
            var config = provider.GetRequiredService<IConfigurationRoot>();

            await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

            var commandStartup = new CommandStartup(_client, host);

            // Subscribe to client log events
            _client.Log += _ => provider.GetRequiredService<ConsoleLogger>().Log(_);
            // Subscribe to slash command log events
            commands.Log += _ => provider.GetRequiredService<ConsoleLogger>().Log(_);

            _client.Ready += Client_Ready;

            _client.Ready += async () =>
            {
                // If running the bot with DEBUG flag, register all commands to guild specified in config
                if (IsDebug())
                    // Id of the test guild can be provided from the Configuration object
                    await commands.RegisterCommandsToGuildAsync(UInt64.Parse(config["testGuild"]), true);
                else
                    // If not debug, register commands globally
                    await commands.RegisterCommandsGloballyAsync(true);
            };


            await _client.LoginAsync(Discord.TokenType.Bot, config["tokens:discord"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
        internal async Task Client_Ready()
        {
            List<ApplicationCommandProperties> applicationCommandProperties = new();
            var guildId = Environment.GetEnvironmentVariable("guildId");
            var guild = _client.GetGuild(UInt64.Parse(guildId));
            // Simple help slash command.
            SlashCommandBuilder globalCommandHelp = new SlashCommandBuilder();
            globalCommandHelp.WithName("help");
            globalCommandHelp.WithDescription("Shows information about the bot.");
            applicationCommandProperties.Add(globalCommandHelp.Build());

            SlashCommandBuilder guildCommandUser = new SlashCommandBuilder();
            guildCommandUser.WithName("user-guild");
            guildCommandUser.WithDescription("GUILD Shows information about the bot.");
            guildCommandUser.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);

            try
            {
                // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
                await guild.CreateApplicationCommandAsync(guildCommandUser.Build());

                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Message, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
            /*await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
        }
        catch (ApplicationCommandException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Message, Formatting.Indented);
            Console.WriteLine(json);
        }*/
        }

        static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}