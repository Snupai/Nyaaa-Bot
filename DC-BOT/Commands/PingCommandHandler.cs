using Discord;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;

namespace DC_BOT.Commands
{
    internal class PingCommandHandler : ICommandHandler
    {
        private readonly ILogger logger;
        private readonly DiscordSocketClient client;

        public PingCommandHandler(ILogger logger, DiscordSocketClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("pong! Latency: pinging...");
            // New LogMessage created to pass desired info to the console using the existing Discord.Net LogMessage parameters
            await logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : Ping", $"User: {command.User.Username}, Command: ping", null));
            // Respond to the user with the latency
            await command.ModifyOriginalResponseAsync(x => x.Content = $"pong! Latency: {client.Latency.ToString()}ms");
        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
                .WithName("ping")
                .WithDescription("Receive a pong!")
                .WithDefaultMemberPermissions(GuildPermission.SendMessages)
                .Build();
        }
    }
}
