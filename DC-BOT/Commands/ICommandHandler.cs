using Discord;
using Discord.WebSocket;

namespace DC_BOT.Commands
{
    internal interface ICommandHandler
    {
        SlashCommandProperties Initialize();

        bool IsGuildCommand { get; }

        Task HandleAsync(SocketSlashCommand command);
    }
}
