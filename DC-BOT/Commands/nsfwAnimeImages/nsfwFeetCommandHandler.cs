using Discord.WebSocket;
using Discord;
using Newtonsoft.Json.Linq;
using System.Net;
using DNet_V3_Tutorial.Log;

namespace DC_BOT.Commands.nsfwAnimeImages
{
    internal class nsfwFeetCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;
        private string apiKey = Environment.GetEnvironmentVariable("apiKey");

        public bool IsGuildCommand => true;

        public nsfwFeetCommandHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(SocketSlashCommand command)
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/nsfw/img/feet";

                await command.RespondAsync("<a:Loading:1087645285628526592> Trying to get a image...");
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
                builder.Description = $"random nsfw feet image";
                builder.ImageUrl = jsonObj.file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : nsfwFeetCommandHandler", $"Bad request, Command: nsfwfeet", null)); //WriteLine($"Error: {e.Message}");
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandnsfwFeet = new SlashCommandBuilder();
            globalCommandnsfwFeet.WithName("nsfwfeet");
            globalCommandnsfwFeet.WithDescription("Sends a random nsfwFeet image.");
            globalCommandnsfwFeet.IsNsfw = true;
            //globalCommandnsfwFeet.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            return globalCommandnsfwFeet.Build();
        }
    }
}
