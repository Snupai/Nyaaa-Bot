using Discord;
using Discord.WebSocket;

namespace DC_BOT.Commands.Neko
{
    internal class NekoCommandHandler : ICommandHandler
    {
        public bool IsGuildCommand => true;

        public async Task HandleAsync(SocketSlashCommand command)
        {
            NekoKind kind;

            foreach (var option in command.Data.Options)
            {
                if (option.Name != "neko") continue;

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
        }

        public SlashCommandProperties Initialize()
        {
            return new SlashCommandBuilder()
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
                )
                .Build();
        }
    }
}
