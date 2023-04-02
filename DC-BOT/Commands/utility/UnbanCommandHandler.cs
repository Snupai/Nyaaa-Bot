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
                throw new Exception("IDK");
                var userName = command.User;
                var thisUser = (SocketGuildUser)command.Data.Options.First().Value;
                //var guild = (SocketGuild)command.GuildLocale.FirstOrDefault();
                var mentionedUser = thisUser.Username;
                var reason = (String)command.Data.Options.Last().Value;
                if (userName.Username == mentionedUser)
                {
                    await command.RespondAsync("You can't unban yourself.\nYou are not even banned...", ephemeral: true);
                    return;
                }

                /*if (thisUser.Hierarchy >= )
                {
                    await command.RespondAsync("Try with a human not a bot.", ephemeral: true);
                    return;
                }*/

                
                //var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                //httpRequest.Headers["Authorization"] = apiKey;
                //await guild.RemoveBanAsync(thisUser);
                
                //var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    result = streamReader.ReadToEnd();
                //}
                //dynamic jsonObj = JObject.Parse(result);

                //string file = jsonObj.file;


                EmbedBuilder builder = new EmbedBuilder();
                builder.Description = $"**{thisUser.Mention}** was unbanned by **{userName.Mention}**\n{reason}";
                //builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await this._logger.Log(new LogMessage(LogSeverity.Info, "CommandHandler : UnbanCommandHandler", $"Bad request {e.Message}, Command: unban", null)); //WriteLine($"Error: {e.Message}");
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandUnban = new SlashCommandBuilder();
            globalCommandUnban.WithName("unban");
            globalCommandUnban.WithDescription("unban someone.");
            globalCommandUnban.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            globalCommandUnban.AddOption("reason", ApplicationCommandOptionType.String, "Define a reason for the ban.");
            return globalCommandUnban.Build();
        }
    }
}