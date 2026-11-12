using Discord.Interactions;

namespace ManorBot.Modules
{
    public class InfoModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("info", "Display information about this app")]
        public async Task InfoAsync()
        {
            await RespondAsync("Hello, World!");
        }
    }
}
