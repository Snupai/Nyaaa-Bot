using Discord;
using Discord.WebSocket;

namespace DC_BOT.Commands.Neko
{
    internal class NekoBoyCommandHandler : ICommandHandler
    {
        private readonly INekoService nekoService;

        public NekoBoyCommandHandler(INekoService nekoService)
        {
            this.nekoService = nekoService ?? throw new ArgumentNullException(nameof(nekoService));
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Trying to get a neko boy...");

            NekoKind kind = NekoKind.NekoBoy;

            var file = this.nekoService.GetNeko(kind);
            await command.ModifyOriginalResponseAsync(x => x.Content = $"{file}");
        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
                .WithName("nekoboy")
                .WithDescription("Receive a sfw neko boy image!")
                .Build();
        }
    }
}
