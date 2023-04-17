using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DC_BOT.Commands
{
    internal class MuteCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;

        public bool IsGuildCommand => true;

        public MuteCommandHandler(ILogger logger)
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
                string time = (String)command.Data.Options.LastOrDefault();

                if (userName.Username == mentionedUser)
                {
                    await command.RespondAsync("You can't mute yourself.", ephemeral: true);
                    return;
                }
                else if (thisUser.Hierarchy >= userName.Hierarchy)
                {
                    await command.RespondAsync("The User you are trying to mute has a higher role than you.", ephemeral: true);
                    return;
                }
                bool yesorno = time.Contains("d");
                if (time.Contains("d")) { time.Split('d'); timespan = new TimeSpan(Convert.ToInt16(time[0]), 0, 0, 0); }
                else if (time.Contains("h")) { time.Split('h'); timespan = new TimeSpan(Convert.ToInt16(time[0]), 0, 0); }
                else if (time.Contains("m")) { time.Split('m'); timespan = new(0, Convert.ToInt16(time[0]), 0); }
                


                await command.RespondAsync("<a:Loading:1087645285628526592> Muting...");

                await thisUser.SetTimeOutAsync(timespan);

                EmbedBuilder builder = new EmbedBuilder();
                builder.Description = $"**{thisUser.Mention}** was mutened by **{userName.Mention}**";
                //builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await this._logger.Log(new LogMessage(LogSeverity.Info, "CommandHandler : MuteCommandHandler", $"Bad request {e.Message}, Command: mute", null)); //WriteLine($"Error: {e.Message}");
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandMute = new SlashCommandBuilder();
            globalCommandMute.WithName("mute");
            globalCommandMute.WithDescription("mute someone.");
            globalCommandMute.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            globalCommandMute.AddOption("time", ApplicationCommandOptionType.String, "Time span to mute ex: 7d 6h 10m", minValue: 0, maxValue: 7);
            //globalCommandMute.AddOption("reason", ApplicationCommandOptionType.String, "Define a reason for the mute.");
            return globalCommandMute.Build();
        }
    }
}
