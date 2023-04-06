using Discord;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json;
using System.Text;

namespace DC_BOT.Commands
{
    internal class AskCommandHandler : ICommandHandler
    {
        private readonly ILogger logger;
        private readonly DiscordSocketClient client;
        private string openAI_api = Environment.GetEnvironmentVariable("OpenAI-api");
        private string openAI_org = Environment.GetEnvironmentVariable("OpenAI-org");

        public AskCommandHandler(ILogger logger, DiscordSocketClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();
            string commanddata = command.Data.Options.First().Value.ToString();
            // New LogMessage created to pass desired info to the console using the existing Discord.Net LogMessage parameters
            // Respond to the user with the latency
            string api_key = "Bearer " + openAI_api;
            var jsonData = await LoadJsonDataFromFileAsync("data.json");
            var manipulatedData = ManipulateData(jsonData, commanddata);


            var response = await PostJsonData("https://api.openai.com/v1/chat/completions", manipulatedData, api_key, openAI_org);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Verarbeitung der Antwort
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
                string messageContent = responseObject.choices[0].message.content;
                await command.ModifyOriginalResponseAsync(x => x.Content = messageContent);
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Fehlerbehandlung
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oopsies...\n{responseContent}");
            }
        }

        public async Task<string> LoadJsonDataFromFileAsync(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public object ManipulateData(string jsonData, string data)
        {
            // Konvertierung der JSON-Daten in ein dynamisches Objekt
            dynamic dyndata = JsonConvert.DeserializeObject(jsonData);

            // Aktualisierung des Inhalts
            dyndata.messages[0].content = data;

            // Rückgabe der manipulierten Daten
            return dyndata;
        }

        public async Task<HttpResponseMessage> PostJsonData(string url, object data, string header1, string header2)
        {
            using (var httpClient = new HttpClient())
            {
                // Adding headers
                httpClient.DefaultRequestHeaders.Add("Authorization", header1);
                httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", header2);

                var jsonData = JsonConvert.SerializeObject(data);
                Console.WriteLine(jsonData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Sending post request
                var response = await httpClient.PostAsync(url, content);

                return response;
            }
        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
                .WithName("ask")
                .WithDescription("ask gpt somethin")
                .WithDefaultMemberPermissions(GuildPermission.SendMessages)
                .AddOption("text", ApplicationCommandOptionType.String, "text for chatgpt", isRequired: true)
                .Build();
        }
    }
}
