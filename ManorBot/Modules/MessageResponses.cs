using Discord.WebSocket;

namespace ManorBot.Modules
{
    internal class MessageResponses
    {
        private static readonly Dictionary<string, string> responses = new()
        {
            ["plonky"] = "https://tenor.com/view/plonk-twitch-cat-gif-27494506"
        };

        public static async Task<string> Respond(SocketMessage msg)
        {
            foreach (var resp in responses)
            {
                if (msg.Content.Contains(resp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    await msg.Channel.SendMessageAsync(resp.Value);
                    return $"responding to keyword \"{resp.Key}\" in channel {msg.Channel.Name}";
                }
            }

            return String.Empty;
        }
    }
}
