﻿using Discord;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DC_BOT.Commands
{
    internal class SmugCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;
        private string apiKey = Environment.GetEnvironmentVariable("apiKey");

        public bool IsGuildCommand => true;

        public SmugCommandHandler(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(SocketSlashCommand command)
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/gif/smug";
                var userName = command.User.Username;
                // var thisUser = (SocketGuildUser)command.Data.Options.First().Value;
                // var mentionedUser = thisUser.Username;
                // if (userName == mentionedUser)
                // {
                //     await command.RespondAsync("Don't slap yourself.", ephemeral: true);
                //     return;
                // }
                // if (thisUser.IsBot)
                // {
                //     await command.RespondAsync("You can't slap a bot.", ephemeral: true);
                //     return;
                // }

                await command.RespondAsync("<a:Loading:1087645285628526592> Trying to get a gif...");
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
                builder.Description = $"**{userName}** smugs";
                builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await this._logger.Log(new LogMessage(LogSeverity.Info, "CommandHandler : SmugCommandHandler", $"Bad request {e.Message}, Command: smug", null)); //WriteLine($"Error: {e.Message}");
                await command.ModifyOriginalResponseAsync(x => x.Content = $"Oops something went wrong.\nPlease try again later.");
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandSmug = new SlashCommandBuilder();
            globalCommandSmug.WithName("smug");
            globalCommandSmug.WithDescription("Smug");
            // globalCommandSmug.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            return globalCommandSmug.Build();
        }
    }
}
