﻿using Discord;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DC_BOT.Commands
{
    internal class CryCommandHandler : ICommandHandler
    {
        private readonly ILogger _logger;
        private string apiKey = Environment.GetEnvironmentVariable("apiKey");

        public bool IsGuildCommand => true;

        public CryCommandHandler(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(SocketSlashCommand command)
        {
            try
            {
                string result;
                var url = "https://gallery.fluxpoint.dev/api/sfw/gif/cry";
                var userName = command.User.Username;
                /*var thisUser = (SocketGuildUser)command.Data.Options.First().Value;
                var mentionedUser = thisUser.Username;
                if (userName == mentionedUser)
                {
                    await command.RespondAsync("Don't bite yourself.", ephemeral: true);
                    return;
                }
                if (thisUser.IsBot)
                {
                    await command.RespondAsync("Try with a human not a bot.", ephemeral: true);
                    return;
                }*/

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
                builder.Description = $"**{userName}** is crying";
                builder.ImageUrl = file;
                builder.Timestamp = DateTime.Now;

                await command.ModifyOriginalResponseAsync(x => x.Content = "\u200D");
                await command.ModifyOriginalResponseAsync(x => x.Embed = builder.Build());

            }
            catch (Exception e)
            {
                await this._logger.Log(new LogMessage(LogSeverity.Info, "CommandHandler : CryCommandHandler", $"Bad request {e.Message}, Command: cry", null)); //WriteLine($"Error: {e.Message}");
                await command.RespondAsync($"Oops something went wrong.\nPlease try again later.", ephemeral: true);
                throw;
            }
        }

        public SlashCommandProperties Initialize()
        {
            SlashCommandBuilder globalCommandCry = new SlashCommandBuilder();
            globalCommandCry.WithName("cry");
            globalCommandCry.WithDescription("Cry.");
            //globalCommandCry.AddOption("user", ApplicationCommandOptionType.User, "Choose a user.", isRequired: true);
            return globalCommandCry.Build();
        }
    }
}
