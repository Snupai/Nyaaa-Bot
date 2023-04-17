using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DC_BOT.Commands
{
    internal class KickCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;

        public bool IsGuildCommand => true;

        public KickCommandHandler(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(SocketSlashCommand command)
        {
            try
            {
                var userName = (SocketGuildUser)command.User;
                var thisUser = (SocketGuildUser)command.Data.Options.First().Value;
                var mentionedUser = thisUser.Username;
                var reason = command.Data.Options.OfType<string>().FirstOrDefault();
                var days = (int)command.Data.Options.OfType<long>().FirstOrDefault();

                if (userName.Username == mentionedUser)
                {
                    await command.RespondAsync("You can't kick yourself.", ephemeral: true);
                    return;
                }
                else if (thisUser.Hierarchy >= userName.Hierarchy)
                {
                    await command.RespondAsync("The User you are trying to kick has a higher role than you.", ephemeral: true);
                    return;
                }

                await command.RespondAsync("<a:Loading:1087645285628526592> Kicking asses...");

                await thisUser.KickAsync(reason);

                EmbedBuilder builder = new EmbedBuilder();
                builder.Description = $"**{thisUser.Mention}** was kicked by **{userName.Mention}**\n{reason}";
                //builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await this._logger.Log(new LogMessage(LogSeverity.Info, "CommandHandler : KickCommandHandler", $"Bad request {e.Message}, Command: kick", null)); //WriteLine($"Error: {e.Message}");
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandKick = new SlashCommandBuilder();
            globalCommandKick.WithName("kick");
            globalCommandKick.WithDescription("kick someone.");
            globalCommandKick.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            globalCommandKick.AddOption("reason", ApplicationCommandOptionType.String, "Define a reason for the ban.");
            return globalCommandKick.Build();
        }
    }
}
