using Discord.WebSocket;
using Discord;
using Newtonsoft.Json.Linq;
using System.Net;
using DNet_V3_Tutorial.Log;

namespace DC_BOT.Commands.nsfwAnimeImages
{
    internal class nsfwAnalCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;
        private string apiKey = Environment.GetEnvironmentVariable("apiKey");

        public bool IsGuildCommand => true;

        public nsfwAnalCommandHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(SocketSlashCommand command)
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/nsfw/img/anal";

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
                builder.Description = $"random nsfw anal image";
                builder.ImageUrl = jsonObj.file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : nsfwAnalCommandHandler", $"Bad request, Command: nsfwanal", null)); //WriteLine($"Error: {e.Message}");
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandnsfwAnal = new SlashCommandBuilder();
            globalCommandnsfwAnal.WithName("nsfwanal");
            globalCommandnsfwAnal.WithDescription("Sends a random nsfwAnal image.");
            globalCommandnsfwAnal.IsNsfw = true;
            //globalCommandnsfwAnal.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            return globalCommandnsfwAnal.Build();
        }
    }
}
