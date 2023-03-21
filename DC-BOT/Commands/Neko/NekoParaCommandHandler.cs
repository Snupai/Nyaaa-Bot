using Discord;
using Discord.WebSocket;

namespace DC_BOT.Commands.Neko
{
    internal class NekoParaCommandHandler : ICommandHandler
    {
        private readonly INekoService nekoService;

        public NekoParaCommandHandler(INekoService nekoService)
        {
            this.nekoService = nekoService ?? throw new ArgumentNullException(nameof(nekoService));
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Trying to get a nekopara...");

            NekoKind kind = NekoKind.NekoPara;

            var file = this.nekoService.GetNeko(kind);
            await command.ModifyOriginalResponseAsync(x => x.Content = $"{file}");
        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
                .WithName("nekopara")
                .WithDescription("Receive a sfw nekopara image!")
                .Build();
        }
    }
}
