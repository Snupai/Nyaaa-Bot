using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DC_BOT.Commands
{
    internal class UnmuteCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;

        public bool IsGuildCommand => true;

        public UnmuteCommandHandler(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(SocketSlashCommand command)
        {
            try
            {
                TimeSpan timespan = new();
                var userName = (SocketGuildUser)command.User;
                var thisUser = (SocketGuildUser)command.Data.Options.First().Value;
                var mentionedUser = thisUser.Username;

                if (userName.Username == mentionedUser)
                {
                    await command.RespondAsync("You can't unmute yourself.", ephemeral: true);
                    return;
                }
                else if (thisUser.Hierarchy >= userName.Hierarchy)
                {
                    await command.RespondAsync("The User you are trying to unmute has a higher role than you.", ephemeral: true);
                    return;
                }
                
                await command.RespondAsync("<a:Loading:1087645285628526592> Unmuting...");

                await thisUser.RemoveTimeOutAsync();

                EmbedBuilder builder = new EmbedBuilder();
                builder.Description = $"**{thisUser.Mention}** was unmuted by **{userName.Mention}**";
                //builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await this._logger.Log(new LogMessage(LogSeverity.Info, "CommandHandler : UnmuteCommandHandler", $"Bad request {e.Message}, Command: unmute", null)); //WriteLine($"Error: {e.Message}");
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandUnmute = new SlashCommandBuilder();
            globalCommandUnmute.WithName("unmute");
            globalCommandUnmute.WithDescription("unmute someone.");
            globalCommandUnmute.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            return globalCommandUnmute.Build();
        }
    }
}
