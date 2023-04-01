using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using System.Security.Cryptography.X509Certificates;

namespace DC_BOT.Commands
{
    internal class HelpCommandHandler : ICommandHandler
    {
        private readonly ILogger logger;
        private readonly DiscordSocketClient client;



        public HelpCommandHandler(ILogger logger, DiscordSocketClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            
            command.RespondAsync("nyot yet implemented\nHere is a bot invite link with admin role.\nhttps://discord.com/api/oauth2/authorize?client_id=1085961992264753202&permissions=8&scope=bot", ephemeral: true);

            
        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
                .WithName("help")
                .WithDescription("Receive help! (nyot yet implemented)")
                .WithDefaultMemberPermissions(GuildPermission.SendMessages)
                .Build();
        }
    }
}
