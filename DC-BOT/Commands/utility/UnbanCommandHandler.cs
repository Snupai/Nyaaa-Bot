using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;
using Discord.Commands;

namespace DC_BOT.Commands
{
    internal class UnbanCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;
        private DiscordSocketClient _client;

        public bool IsGuildCommand => true;

        public UnbanCommandHandler(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(SocketSlashCommand command)
        {
            try
            {
                await command.RespondAsync("<a:Loading:1087645285628526592> Unbanning...");
                var userName = (SocketGuildUser)command.User;
                var thisUser = ulong.Parse((string)command.Data.Options.First().Value);
                
                await userName.Guild.RemoveBanAsync(thisUser);

                EmbedBuilder builder = new EmbedBuilder();
                builder.Description = $"**<@{thisUser}>** was unbanned by **{userName.Mention}**";
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());
            }
            catch (Exception e)
            {
                await this._logger.Log(new LogMessage(LogSeverity.Info, "CommandHandler : UnbanCommandHandler", $"Bad request {e.Message}, Command: unban", null));
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandUnban = new SlashCommandBuilder();
            globalCommandUnban.WithName("unban");
            globalCommandUnban.WithDescription("unban someone.");
            globalCommandUnban.AddOption("user-id", ApplicationCommandOptionType.String, "Id of the user to unban.", isRequired: true);
            return globalCommandUnban.Build();
        }
    }
}