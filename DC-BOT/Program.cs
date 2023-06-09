﻿using DC_BOT.Commands;
using DC_BOT.Commands.AnimeImages;
using DC_BOT.Commands.Interactions;
using DC_BOT.Commands.Neko;
using DC_BOT.Commands.nsfwAnimeImages;
using DC_BOT.Commands.nsfwInteractions;
using DC_BOT.Commands.Utility;
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
            .SetBasePath(AppContext.BaseDirectory)
            .AddYamlFile("config.yml")
            .Build();
            Environment.SetEnvironmentVariable("apiKey", config["tokens:fluxpoint-api"]);
            Environment.SetEnvironmentVariable("guildId", config["testGuild"]);
            Environment.SetEnvironmentVariable("OpenAI-org", config["tokens:openai-org"]);
            Environment.SetEnvironmentVariable("OpenAI-api", config["tokens:openai-api"]);
            Environment.SetEnvironmentVariable("BingChat-cookie", config["tokens:bing-cookie"]);
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
                    .AddSingleton<ICommandHandler, FemboyCommandHandler>()
                    .AddSingleton<ICommandHandler, ZeroTwoCommandHandler>()
                    .AddSingleton<ICommandHandler, PingCommandHandler>()
                    .AddSingleton<ICommandHandler, HelpCommandHandler>()
                    .AddSingleton<ICommandHandler, AskCommandHandler>()
                    .AddSingleton<ICommandHandler, BingCommandHandler>()
                    .AddNekoCommands()
                    .AddAnimeImageCommands()
                    .AddInteractionCommands()
                    .AddnsfwAnimeImageCommands()
                    .AddnsfwInteractionCommands()
                    .AddUtilityCommands()
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

            if (migrate)
            {
                Console.WriteLine("Do you also want to delete old commands? If so type 'yes'");
                var shouldDelete = Console.ReadLine() == "yes";

                CommandStartup.ShouldDelete = shouldDelete;
            }

            if (migrate)
            {
                _client.Ready += commandStartup.MigrateGuildCommands;
            }

            await _client.LoginAsync(Discord.TokenType.Bot, config["tokens:discord"]);
            await _client.StartAsync();
            await commandStartup.Start();

            if (migrate)
            {
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