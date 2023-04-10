using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DC_BOT.Commands
{
    internal class BanCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;

        public bool IsGuildCommand => true;

        public BanCommandHandler(ILogger logger)
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
                    await command.RespondAsync("You can't ban yourself.", ephemeral: true);
                    return;
                }
                else if (thisUser.Hierarchy >= userName.Hierarchy)
                {
                    await command.RespondAsync("The User you are trying to ban has a higher role than you.", ephemeral: true);
                    return;
                }

                await command.RespondAsync("<a:Loading:1087645285628526592> Banning...");

                await userName.Guild.AddBanAsync(thisUser, days, reason);

                EmbedBuilder builder = new EmbedBuilder();
                builder.Description = $"**{thisUser.Mention}** was banned by **{userName.Mention}**\n{reason}";
                //builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await this._logger.Log(new LogMessage(LogSeverity.Info, "CommandHandler : BanCommandHandler", $"Bad request {e.Message}, Command: ban", null)); //WriteLine($"Error: {e.Message}");
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandBan = new SlashCommandBuilder();
            globalCommandBan.WithName("ban");
            globalCommandBan.WithDescription("ban someone.");
            globalCommandBan.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            globalCommandBan.AddOption("time", ApplicationCommandOptionType.Integer, "The number of days of the banned user's messages to purge.", minValue: 0, maxValue: 7);
            globalCommandBan.AddOption("reason", ApplicationCommandOptionType.String, "Define a reason for the ban.");
            return globalCommandBan.Build();
        }
    }
}
