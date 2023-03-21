using DC_BOT.Commands;
using DC_BOT.Commands.Neko;
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
                        UseInteractionSnowflakeDate = false,
                        LogLevel = LogSeverity.Debug
                    }))
                    // Adding console logging
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
                    .AddSingleton<ICommandHandler, BiteCommandHandler>()
                    .AddSingleton<ICommandHandler, BlushCommandHandler>()
                    .AddSingleton<ICommandHandler, CryCommandHandler>()
                    .AddSingleton<ICommandHandler, DanceCommandHandler>()
                    .AddSingleton<ICommandHandler, FeedCommandHandler>()
                    .AddSingleton<ICommandHandler, FluffCommandHandler>()
                    .AddSingleton<ICommandHandler, GrabCheeksCommandHandler>()
                    .AddSingleton<ICommandHandler, HandHoldCommandHandler>()
                    .AddSingleton<ICommandHandler, HighfiveCommandHandler>()
                    .AddSingleton<ICommandHandler, HugCommandHandler>()
                    .AddSingleton<ICommandHandler, KissCommandHandler>()
                    .AddSingleton<ICommandHandler, LickCommandHandler>()
                    .AddSingleton<ICommandHandler, PatCommandHandler>()
                    .AddSingleton<ICommandHandler, PokeCommandHandler>()
                    .AddSingleton<ICommandHandler, PunchCommandHandler>()
                    .AddSingleton<ICommandHandler, ZeroTwoCommandHandler>()
                    .AddSingleton<ICommandHandler, PingCommandHandler>()
                    .AddSingleton<ICommandHandler, SlapCommandHandler>()
                    .AddSingleton<ICommandHandler, ShrugCommandHandler>()
                    .AddSingleton<ICommandHandler, SmugCommandHandler>()
                    .AddSingleton<ICommandHandler, StareCommandHandler>()
                    .AddSingleton<ICommandHandler, WagCommandHandler>()
                    .AddSingleton<ICommandHandler, TickleCommandHandler>()
                    .AddSingleton<ICommandHandler, WaveCommandHandler>()
                    .AddSingleton<ICommandHandler, WinkCommandHandler>()
                    .AddSingleton<ICommandHandler, AnimeCommandHandler>()
                    .AddSingleton<ICommandHandler, AzurlaneCommandHandler>()
                    .AddSingleton<ICommandHandler, ChibiCommandHandler>()
                    .AddSingleton<ICommandHandler, ChristmasCommandHandler>()
                    .AddSingleton<ICommandHandler, DDLCCommandHandler>()
                    .AddSingleton<ICommandHandler, HalloweenCommandHandler>()
                    .AddSingleton<ICommandHandler, HoloCommandHandler>()
                    .AddSingleton<ICommandHandler, KitsuneCommandHandler>()
                    .AddSingleton<ICommandHandler, MaidCommandHandler>()
                    .AddSingleton<ICommandHandler, SenkoCommandHandler>()
                    .AddNekoCommands()
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

            var logger = provider.GetRequiredService<ILogger>();

            // Subscribe to client log events
            _client.Log += _ => logger.Log(_);
            // Subscribe to slash command log events
            commands.Log += _ => logger.Log(_);

            // _client.Ready += Client_Ready;

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

            var commandStartup = new CommandStartup(_client, host);

            Console.WriteLine("Do you want to migrate commands? If so type 'yes'");

            var response = Console.ReadLine();
            bool migrate = response == "yes";

            if (migrate) {
                Console.WriteLine("Do you also want to delete old commands? If so type 'yes'");
                var shouldDelete = Console.ReadLine() == "yes";

                CommandStartup.ShouldDelete = shouldDelete;
            }

            if (migrate) {
                _client.Ready += commandStartup.MigrateGuildCommands;
            }

            await _client.LoginAsync(Discord.TokenType.Bot, config["tokens:discord"]);
            await _client.StartAsync();
            await commandStartup.Start();

            if (migrate) {
                await commandStartup.MigrateCommands();
            }

            await _client.SetStatusAsync(UserStatus.Idle);

            await Task.Delay(-1);
        }
/*
        public async Task Client_Ready()
        {
            List<ApplicationCommandProperties> applicationCommandProperties = new();
            try
            {
                SlashCommandBuilder globalCommandHelp = new SlashCommandBuilder();
                globalCommandHelp.WithName("help");
                globalCommandHelp.WithDescription("Shows information about the bot.");
                applicationCommandProperties.Add(globalCommandHelp.Build());
                await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
            }
            catch (Exception ex)
            {
                throw;
            }
        }
*/
        static bool IsDebug()
        {
#if !DEBUG
            return true;
#else 
            return false;
#endif
        }
    }
}