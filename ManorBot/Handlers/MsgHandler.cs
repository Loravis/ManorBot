using Discord.WebSocket;
using System.Diagnostics;
using ManorBot.Modules;
using ManorBot.Diagnostics;

namespace ManorBot.Handlers
{
    internal class MsgHandler(TaskTimer timer)
    {
        private readonly Logger logger = new("MsgHandler");
        public async Task HandleAsync(SocketMessage msg)
        {
            if (msg is not SocketUserMessage) return;

            timer.Start();

            string workDescription = await MessageResponses.Respond(msg);
            if (string.IsNullOrEmpty(workDescription)) return;

            await timer.Stop(workDescription);
        }
    }
}
