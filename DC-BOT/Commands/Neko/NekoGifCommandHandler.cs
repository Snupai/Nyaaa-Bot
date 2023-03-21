using Discord;
using Discord.WebSocket;

namespace DC_BOT.Commands.Neko
{
    internal class NekoGifCommandHandler : ICommandHandler
    {
        private readonly INekoService nekoService;

        public NekoGifCommandHandler(INekoService nekoService)
        {
            this.nekoService = nekoService ?? throw new ArgumentNullException(nameof(nekoService));
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Trying to get a gif...");

            NekoKind kind = NekoKind.NekoGif;

            var file = this.nekoService.GetNeko(kind);
            await command.ModifyOriginalResponseAsync(x => x.Content = $"{file}");
        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
                .WithName("nekogif")
                .WithDescription("Receive a sfw neko gif!")
                .Build();
        }
    }
}
