using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DNet_V3_Tutorial.Log;

namespace DNet_V3_Tutorial
{
    // Must use InteractionModuleBase<SocketInteractionContext> for the InteractionService to auto-register the commands
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands { get; set; }
        private static Logger _logger;

        private readonly ulong roleID = 735934868260913498;

        public InteractionModule(ConsoleLogger logger)
        {
            _logger = logger;
        }


        // Basic slash command. [SlashCommand("name", "description")]
        // Similar to text command creation, and their respective attributes
        [SlashCommand("ping", "Receive a pong!")]
        public async Task Ping()
        {
            // New LogMessage created to pass desired info to the console using the existing Discord.Net LogMessage parameters
            await _logger.Log(new LogMessage(LogSeverity.Info, "PingModule : Ping", $"User: {Context.User.Username}, Command: ping", null));
            // Get the current latency
            var latency = Context.Client.Latency;
            // Respond to the user with the latency
            await RespondAsync($"pong! Latency: {latency.ToString()}ms");
        }

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

        [UserCommand("give-role")]
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
