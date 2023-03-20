using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DNet_V3_Tutorial
{
    // Must use InteractionModuleBase<SocketInteractionContext> for the InteractionService to auto-register the commands
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        string apiKey = Environment.GetEnvironmentVariable("apiKey");

        public InteractionService Commands { get; set; }
        private static ILogger _logger;

        private readonly ulong roleID = 735934868260913498;

        public InteractionModule(ILogger logger)
        {
            _logger = logger;
        }

        //-------------------------------------------------------------------------------------------------------------------
        // Here starts reactions responses
        //-------------------------------------------------------------------------------------------------------------------

        //[SlashCommand("baka", "Say baka to someone!")]
        public async Task sfwReactBakaGif(string guildUser = "")
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/gif/baka";
                var userName = Context.User.Username;
                //var guildUser = "";      
                if (userName == guildUser)
                {
                    await RespondAsync("Don't call yourself an idiot.", ephemeral: true);
                    return;
                }

                await RespondAsync("Trying to get a gif...");
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
                builder.Description = $"**{userName}** calls **{guildUser}** an idiot";
                builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwReactBakaGif", $"Bad request {e.Message}, Command: baka", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------
        // reactions responses ends here
        //-------------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------------
        // Here starts neko image responses
        //-------------------------------------------------------------------------------------------------------------------

        [SlashCommand("neko", "Receive a sfw neko image or gif!")]
        public async Task sfwNekoImgGif([Choice("Img", "img"), Choice("Gif", "gif"), Choice("Neko-boy", "nekoboy")] string type)
        {
            try
            {
                await RespondAsync("Trying to get a gif...");
                string result;
                var url = "";
                if (type == "img")
                {
                    url = "https://gallery.fluxpoint.dev/api/sfw/img/neko";
                }
                else if (type == "gif")
                {
                    url = "https://gallery.fluxpoint.dev/api/sfw/gif/neko";
                }
                else if (type == "nekoboy")
                {
                    url = "https://gallery.fluxpoint.dev/api/sfw/img/nekoboy";
                }
                else
                {
                    await RespondAsync($"Please specify what you want.\nUsage: `/neko [img/gif/neko-boy]`", ephemeral: true);
                    return;
                }

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwNekoImgGif", $"Bad request, Command: neko", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("neko-boy", "Receive a sfw neko boy image!")]
        public async Task sfwNekoBoyImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/nekoboy";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwNekoBoyImg", $"Bad request, Command: neko-boy", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("nekopara", "Receive a sfw nekopara image!")]
        public async Task sfwNekoparaImg()
        {
            await RespondAsync("Trying to get a gif...");
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/nekopara";

                
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwNekoparaImg", $"Bad request, Command: nekopara", null)); //WriteLine($"Error: {e.Message}");
                await ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------
        // neko image responses ends here
        //-------------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------------
        // Here starts fox/kistune image responses
        //-------------------------------------------------------------------------------------------------------------------

        [SlashCommand("senko", "Receive a sfw senko image!")]
        public async Task sfwSenkoImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/senko";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwSenkoImg", $"Bad request, Command: senko", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("kitsune", "Receive a sfw kitsune image!")]
        public async Task sfwKitsuneImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/kitsune";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwKitsuneImg", $"Bad request, Command: kitsune", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("holo", "Receive a sfw holo kitsune image!")]
        public async Task sfwHoloImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/holo";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwHoloImg", $"Bad request, Command: holo", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------
        // fox/kistune image responses ends here
        //-------------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------------
        // Here starts anime image responses
        //-------------------------------------------------------------------------------------------------------------------

        [SlashCommand("anime", "Receive a sfw anime image!")]
        public async Task sfwAnimeImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/anime";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwAnimeImg", $"Bad request, Command: anime", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("maid", "Receive a sfw maid image!")]
        public async Task sfwMaidImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/maid";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwMaidImg", $"Bad request, Command: maid", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("halloween", "Receive a sfw halloween image!")]
        public async Task sfwHalloweenImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/halloween";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwHalloweenImg", $"Bad request, Command: halloween", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("ddlc", "Receive a sfw Doki Doki Literature Club image!")]
        public async Task sfwDDLCImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/ddlc";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwDDLCImg", $"Bad request, Command: ddlc", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("christmas", "Receive a sfw christmas image!")]
        public async Task sfwChristmasImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/christmas";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwChristmasImg", $"Bad request, Command: christmas", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("chibi", "Receive a sfw chibi image!")]
        public async Task sfwChibiImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/chibi";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwChibiImg", $"Bad request, Command: chibi", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        [SlashCommand("azurlane", "Receive a sfw azurlane image!")]
        public async Task sfwAzurlaneImg()
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/img/azurlane";

                await RespondAsync("Trying to get a gif...");
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers["Authorization"] = apiKey;


                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                dynamic jsonObj = JObject.Parse(result);

                string file = jsonObj.file;
                await ModifyOriginalResponseAsync(x => x.Content = $"{file}");
            }
            catch (Exception e)
            {
                await _logger.Log(new LogMessage(LogSeverity.Info, "InteractionModule : sfwAzurlaneImg", $"Bad request, Command: azurlane", null)); //WriteLine($"Error: {e.Message}");
                await RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------
        // anime image responses ends here
        //-------------------------------------------------------------------------------------------------------------------

        [SlashCommand("components", "Demonstrate buttons and select menus.")]
        public async Task HandleComponementCommand()
        {
            var button = new ButtonBuilder()
            {
                Label = "Button!",
                CustomId = "button",
                Style = ButtonStyle.Primary
            };

            var menu = new SelectMenuBuilder()
            {
                CustomId = "menu",
                Placeholder = "Sample Menu"
            };
            menu.AddOption("First Option", "first");
            menu.AddOption("Second Option", "second");

            var component = new ComponentBuilder();
            component.WithButton(button);
            component.WithSelectMenu(menu);

            await RespondAsync("testing", components: component.Build());
        }

        [ComponentInteraction("button")]
        public async Task HandleButtonInput()
        {
            await RespondWithModalAsync<DemoModal>("demo_modal");
        }

        [ComponentInteraction("menu")]
        public async Task HandleMenuSelection(string[] inputs)
        {
            await RespondAsync(inputs[0]);
        }

        [ModalInteraction("demo_modal")]
        public async Task HandleModalInput(DemoModal modal)
        {
            string input = modal.Greeting;
            await RespondAsync(input);
        }

        [UserCommand("give-role Member")]
        public async Task HandleUserCommand(IUser user)
        {
            await (user as SocketGuildUser).AddRoleAsync(roleID);
            var roles = (user as SocketGuildUser).Roles;
            string rolesList = string.Empty;
            foreach (var role in roles)
            {
                rolesList += role.Name + "\n";
            }

            await RespondAsync($"User {user.Mention} has the following roles:\n" + rolesList);
        }

        [MessageCommand("msg-command")]
        public async Task HandleMessageCommand(IMessage message)
        {
            await RespondAsync($"Message author is: {message.Author.Username}");
        }
    }

    public class DemoModal : IModal
    {
        public string Title => "Demo Modal";
        [InputLabel("Send a greeting!")]
        [ModalTextInput("greeting_input", TextInputStyle.Short, placeholder: "Be nice...", maxLength: 100)]
        public string Greeting { get; set; }
    }
}
