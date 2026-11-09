using Discord.WebSocket;
using ManorBot.Modules;
using ManorBot.Diagnostics;

namespace ManorBot.Handlers
{
    /// <summary>
    /// Handles Discord channel messages provided by the <c>DiscordSocketClient.MessageReceived</c> event.
    /// This class processes incoming messages and delegates to appropriate response handlers.
    /// </summary>
    /// <remarks>
    /// Do not use this class to implement commands!
    /// Implement slash commands instead and implement them in a separate class.
    /// </remarks>
    internal class MsgHandler(TaskTimer timer, Logger logger)
    {
        private readonly MessageResponses messageResponses = new(logger);

        /// <summary>
        /// Processes a Discord message. All methods used to track sent messages should be called here.
        /// This method should only be called by the MessageReceived event unless there's another reason to call it,
        /// which should be clearly documented.
        /// </summary>
        /// <param name="msg">The SocketMessage to be processed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HandleAsync(SocketMessage msg)
        {
            if (msg is not SocketUserMessage) return;

            timer.Start();

            string workDescription = await messageResponses.Respond(msg);
            if (string.IsNullOrEmpty(workDescription)) return;

            await timer.Stop(workDescription);
        }
    }
}
