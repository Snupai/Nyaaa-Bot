using Discord;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DC_BOT.Commands
{
    internal class BakaCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;
        private string apiKey = Environment.GetEnvironmentVariable("apiKey");

        public BakaCommandHandler(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(SocketSlashCommand command)
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/gif/baka";
                var userName = command.User.Username;
                var thisUser = (SocketGuildUser)command.Data.Options.First().Value;
                var mentionedUser = thisUser.Username;
                if (userName == mentionedUser)
                {
                    await command.RespondAsync("Don't call yourself an idiot.", ephemeral: true);
                    return;
                }

                await command.RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;


                EmbedBuilder builder = new EmbedBuilder();
                builder.Description = $"**{userName}** calls **{mentionedUser}** an idiot";
                builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await this._logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwReactBakaGif", $"Bad request {e.Message}, Command: baka", null)); //WriteLine($"Error: {e.Message}");
                await command.RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }

            //// We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
            //var guildUser = (SocketGuildUser)command.Data.Options.First().Value;

            //// We remove the everyone role and select the mention of each role.
            //var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            //var embedBuiler = new EmbedBuilder()
            //    .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
            //    .WithTitle("Roles")
            //    .WithDescription(roleList)
            //    .WithColor(Color.Green)
            //    .WithCurrentTimestamp();

            //// Now, Let's respond with the embed.
            //await command.RespondAsync(embed: embedBuiler.Build());
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandUser = new SlashCommandBuilder();
            globalCommandUser.WithName("baka");
            globalCommandUser.WithDescription("Shows information about the bot.");
            globalCommandUser.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            return globalCommandUser.Build();
        }
    }
}
