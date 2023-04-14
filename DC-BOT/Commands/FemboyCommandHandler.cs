using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Web;

namespace DC_BOT.Commands
{
    internal class FemboyCommandHandler : ICommandHandler
    {
        const string r34baseUrl = "https://api.rule34.xxx/index.php";
        const string realbooruBaseUrl = "https://realbooru.com/index.php";
        const int r34knownMaximum = 118816;
        const int realbooruKnownMaximum = 3942;

        private readonly ILogger logger;

        public FemboyCommandHandler(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();
            var type = command.Data.Options.LastOrDefault();

            using (HttpClient client = new HttpClient())
            {
                var r = new Random();

                if (type.Value.ToString() == "r34")
                {
                    UriBuilder urlBuilder = new UriBuilder(r34baseUrl);
                    var parameters = HttpUtility.ParseQueryString(string.Empty);
                    parameters["page"] = "dapi";
                    parameters["s"] = "post";
                    parameters["q"] = "index";
                    parameters["json"] = "1";
                    parameters["tags"] = "femboy";
                    parameters["limit"] = "1";
                    parameters["pid"] = (r.NextInt64() % r34knownMaximum).ToString();
                    urlBuilder.Query = parameters.ToString();

                    var url = urlBuilder.Uri;
                    Console.WriteLine(url);
                    var res = await client.GetAsync(url);
                    var stringRes = await res.Content.ReadAsStringAsync();
                    var rule34Response = JsonConvert.DeserializeObject<List<Rule34Response>>(stringRes)[0];

                    EmbedBuilder builder = new EmbedBuilder();
                    builder.Description = $"Femboy <:AstolfoSugoi:698271845689983057>";
                    builder.ImageUrl = rule34Response.FileUrl;
                    builder.Timestamp = DateTime.Now;

                    await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                    await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());
                }
                else if (type.Value.ToString() == "realbooru")
                {
                    UriBuilder urlBuilder = new UriBuilder(realbooruBaseUrl);
                    var parameters = HttpUtility.ParseQueryString(string.Empty);
                    parameters["page"] = "dapi";
                    parameters["s"] = "post";
                    parameters["q"] = "index";
                    parameters["json"] = "1";
                    parameters["tags"] = "femboy+-rating:explicit";
                    parameters["limit"] = "1";
                    parameters["pid"] = (r.NextInt64() % realbooruKnownMaximum).ToString();
                    urlBuilder.Query = parameters.ToString();

                    var url = urlBuilder.Uri;
                    Console.WriteLine(url);
                    var res = await client.GetAsync(url);
                    var stringRes = await res.Content.ReadAsStringAsync();
                    var realbooruResponse = JsonConvert.DeserializeObject<List<RealbooruResponse>>(stringRes)[0];

                    EmbedBuilder builder = new EmbedBuilder();
                    builder.Description = $"Femboy <:AstolfoSugoi:698271845689983057>";
                    builder.ImageUrl = realbooruResponse.FileUrl;
                    builder.Timestamp = DateTime.Now;

                    await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                    await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());
                }
                else
                {
                    await command.ModifyOriginalResponseAsync(x => x.Content = type.Value.ToString());
                    throw new Exception("uhhh weird");
                }
            }
        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
                .WithName("femboy")
                .WithDescription("Get a random femboy image")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("tag")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithDescription("r34 tag or list-all")
                    .AddChoice("Rule 34", "r34")
                    .AddChoice("RealBooru", "realbooru")
                    .WithRequired(true)
                )
                .WithNsfw(true)
                .Build();
        }

        private class Rule34Response
        {
            [JsonProperty(PropertyName = "file_url")]
            public string FileUrl { get; set; }

            [JsonProperty(PropertyName = "owner")]
            public string Owner { get; set; }
        }

        private class RealbooruResponse
        {
            [JsonProperty(PropertyName = "file_url")]
            public string FileUrl { get; set; }

            [JsonProperty(PropertyName = "owner")]
            public string Owner { get; set; }
        }
    }
}