using Discord;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using BingChat;

namespace DC_BOT.Commands
{
    internal class BingCommandHandler : ICommandHandler
    {
        private readonly ILogger logger;
        private readonly DiscordSocketClient client;
        private string BingChat_cookie = Environment.GetEnvironmentVariable("BingChat-cookie");

        public BingCommandHandler(ILogger logger, DiscordSocketClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();
            string message = command.Data.Options.First().Value.ToString();

            // New LogMessage created to pass desired info to the console using the existing Discord.Net LogMessage parameters
            await logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : Bing", $"User: {command.User.Username}, Command: bing", null));
            // Respond to the user with the latency
            // Construct the chat client
            var client = new BingChatClient(new BingChatClientOptions
            {
                // The "_U" cookie's value
                Cookie = BingChat_cookie
            });
            var answer = await client.AskAsync(message);


            await command.ModifyOriginalResponseAsync(x => x.Content = $"{answer}");
        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
                .WithName("bing")
                .WithDescription("ask bing somethin")
                .WithDefaultMemberPermissions(GuildPermission.SendMessages)
                .AddOption("text", ApplicationCommandOptionType.String, "text for bing", isRequired: true)
                .Build();
        }
    }
}
