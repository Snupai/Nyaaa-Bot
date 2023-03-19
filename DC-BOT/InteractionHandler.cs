using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Reflection;

namespace DNet_V3_Tutorial
{
    public class InteractionHandler
    {
        string apiKey = Environment.GetEnvironmentVariable("apiKey");

        public InteractionService Commands { get; set; }
        private static Logger _logger;

        
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;

        // Using constructor injection
        public InteractionHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services, ConsoleLogger logger)
        {
            _client = client;
            _commands = commands;
            _services = services;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += HandleInteraction;
            
            // Process the command execution results 
            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
            _client.SlashCommandExecuted += SlashCommandHandler;
        }

        private Task ComponentCommandExecuted(ComponentCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            
            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            return Task.CompletedTask;
        }

        private Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {   
            return Task.CompletedTask;
        }
        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(ctx, _services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "baka":
                    await HandleListRoleCommand(command);
                    break;
            }
        }

        private async Task HandleListRoleCommand(SocketSlashCommand command)
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/gif/baka";
                var userName = command.User.Username;
                var thisUser = (SocketGuildUser)command.Data.Options.First().Value;
                var mentionedUser = thisUser.Username;
                if (userName == mentionedUser)
                {
                    await command.RespondAsync("Don't call yourself an idiot.", ephemeral: true);
                    return;
                }

                await command.RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;


                EmbedBuilder builder = new EmbedBuilder();
                builder.Description = $"**{userName}** calls **{mentionedUser}** an idiot";
                builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwReactBakaGif", $"Bad request {e.Message}, Command: baka", null)); //WriteLine($"Error: {e.Message}");
                await command.RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }

            //// We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
            //var guildUser = (SocketGuildUser)command.Data.Options.First().Value;

            //// We remove the everyone role and select the mention of each role.
            //var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            //var embedBuiler = new EmbedBuilder()
            //    .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
            //    .WithTitle("Roles")
            //    .WithDescription(roleList)
            //    .WithColor(Color.Green)
            //    .WithCurrentTimestamp();

            //// Now, Let's respond with the embed.
            //await command.RespondAsync(embed: embedBuiler.Build());
        }
    }
}
