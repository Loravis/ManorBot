using Discord.WebSocket;
using System.Diagnostics;
using ManorBot.Modules;

namespace ManorBot.Handlers
{
    internal class MsgHandler()
    {
        private readonly Logger logger = new("MsgHandler");
        public async Task HandleAsync(SocketMessage msg)
        {
            if (msg is not SocketUserMessage)
                return;

            long startTime = Stopwatch.GetTimestamp();

            string workDescription = await MessageResponses.Respond(msg);
            
            if (workDescription == String.Empty)
                return;

            TimeSpan elapsedTime = Stopwatch.GetElapsedTime(startTime);
            int time = elapsedTime.Microseconds;

            if (time > 999)
            {
                await logger.Log($"Finished {workDescription} in {time / 1000}ms");
            }

            else
            {
                await logger.Log($"Finished {workDescription} in {time}µs");
            }
        }
    }
}
