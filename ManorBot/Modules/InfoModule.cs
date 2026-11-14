using Discord;
using Discord.Interactions;

namespace ManorBot.Modules
{
    public class InfoModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("info", "Display information about this app")]
        public async Task InfoAsync()
        {
            var embed = new EmbedBuilder
            {
                Title = "Hello, World!",
                Description = "This is a test.",
                Color = Color.Blue
            };
            
            await RespondAsync(embed: embed.Build());
        }
    }
}
