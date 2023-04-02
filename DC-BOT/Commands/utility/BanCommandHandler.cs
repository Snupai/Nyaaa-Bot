using Discord;
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
                var userName = command.User;
                var thisUser = (SocketGuildUser)command.Data.Options.First().Value;
                var mentionedUser = thisUser.Username;
                var reason = (String)command.Data.Options.Last().Value;
                if (userName.Username == mentionedUser)
                {
                    await command.RespondAsync("You can't ban yourself.", ephemeral: true);
                    return;
                }
                
                /*if (thisUser.Hierarchy >= )
                {
                    await command.RespondAsync("Try with a human not a bot.", ephemeral: true);
                    return;
                }*/

                await command.RespondAsync("<a:Loading:1087645285628526592> Banning...");
                //var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                //httpRequest.Headers["Authorization"] = apiKey;
                await thisUser.Guild.AddBanAsync(thisUser, 0, reason);

                //var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    result = streamReader.ReadToEnd();
                //}
                //dynamic jsonObj = JObject.Parse(result);

                //string file = jsonObj.file;


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
            globalCommandBan.AddOption("reason", ApplicationCommandOptionType.String, "Define a reason for the ban.");
            return globalCommandBan.Build();
        }
    }
}
