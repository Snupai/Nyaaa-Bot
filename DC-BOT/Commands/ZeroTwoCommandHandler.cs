using Discord;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json;
using System.Web;

namespace DC_BOT.Commands
{
    internal class ZeroTwoCommandHandler : ICommandHandler
    {
        const string baseUrl = "https://api.rule34.xxx/index.php";
        const int knownMaximum = 2281;

        private readonly ILogger logger;

        public ZeroTwoCommandHandler(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();


            using (HttpClient client = new HttpClient()) {
                var r = new Random();

                UriBuilder urlBuilder = new UriBuilder(baseUrl);
                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters["page"] = "dapi";
                parameters["s"] = "post";
                parameters["q"] = "index";
                parameters["json"] = "1";
                parameters["tags"] = "zero_two_(darling_in_the_franxx)";
                parameters["limit"] = "1";
                parameters["pid"] = (r.NextInt64() % knownMaximum).ToString();
                urlBuilder.Query = parameters.ToString();

                var url = urlBuilder.Uri;
                Console.WriteLine(url);
                var res = await client.GetAsync(url);
                var stringRes = await res.Content.ReadAsStringAsync();
                var rule34Response = JsonConvert.DeserializeObject<List<Rule34Response>>(stringRes)[0];

                EmbedBuilder builder = new EmbedBuilder();
                builder.Description = $"Zero two thicc <a:02Nod:734489242985693266>";
                builder.ImageUrl = rule34Response.FileUrl;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());
            }

        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
                .WithName("zerotwo")
                .WithDescription("Get a random zero two image from r34")
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
    }

    
}
