using Discord;
using Discord.WebSocket;

namespace DC_BOT.Commands.Neko
{
    internal class NekoCommandHandler : ICommandHandler
    {
        private readonly INekoService nekoService;

        public NekoCommandHandler(INekoService nekoService)
        {
            this.nekoService = nekoService ?? throw new ArgumentNullException(nameof(nekoService));
        }

        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            NekoKind kind = NekoKind.Neko;
            string kindStr = "neko";

            foreach (var option in command.Data.Options)
            {
                if (option.Name != "kind") continue;
                kindStr = (string) option.Value;
                switch (option.Value) {
                    case "neko":
                        kind = NekoKind.Neko;
                        break;
                    case "boy":
                        kind = NekoKind.NekoBoy;
                        break;
                    case "gif":
                        kind = NekoKind.NekoGif;
                        break;
                    case "nekopara":
                        kind = NekoKind.NekoPara;
                        break;
                    default:
                        throw new Exception();
                }
            }

            await command.RespondAsync($"Trying to get a {kindStr}...");

            var file = this.nekoService.GetNeko(kind);
            await command.ModifyOriginalResponseAsync(x => x.Content = $"{file}");
        }

        public SlashCommandProperties Initialize()
        {
            var command = new SlashCommandBuilder()
                .WithName("neko")
                .WithDescription("Receive a sfw neko image or gif!")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("kind")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithDescription("pog")
                    .AddChoice("Neko", "neko")
                    .AddChoice("Neko boy", "boy")
                    .AddChoice("Neko gif", "gif")
                    .AddChoice("Nekopara", "nekopara")
                    .WithRequired(false)
                )
                .Build();
            return command;
        }
    }
}
