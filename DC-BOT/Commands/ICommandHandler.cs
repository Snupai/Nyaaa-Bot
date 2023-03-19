using Discord;
using Discord.WebSocket;

namespace DC_BOT.Commands
{
    internal interface ICommandHandler
    {
        SlashCommandProperties Initialize();

        Task HandleAsync(SocketSlashCommand command);
    }
}
