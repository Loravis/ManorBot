using Discord.WebSocket;

namespace ManorBot.Modules
{
    /// <summary>
    /// Handles automatic message responses based on keyword matching.
    /// This module scans incoming messages for predefined keywords and responds with configured content.
    /// </summary>
    internal class MessageResponses
    {
        /// <summary>
        /// Dictionary mapping keywords to their corresponding response content.
        /// Keyword matching is case-insensitive.
        /// </summary>
        private static readonly Dictionary<string, string> responses = new()
        {
            ["plonky"] = "https://tenor.com/view/plonk-twitch-cat-gif-27494506"
        };

        /// <summary>
        /// Processes a message and responds if any configured keywords are found.
        /// The search is case-insensitive and sends the response to the same channel as the incoming message.
        /// </summary>
        /// <param name="msg">The Discord message to check for keywords.</param>
        /// <returns>
        /// A string describing the work performed ("responding to keyword X in channel Y"), or
        /// an empty string if no keywords matched and no response was sent.
        /// </returns>
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
