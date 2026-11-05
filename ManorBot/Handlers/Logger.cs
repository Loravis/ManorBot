using Discord;

namespace ManorBot.Handlers
{
    internal class Logger(string name)
    {
        private static Task LogInternal(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        public Task Log(string msg)
        {
            var logMessage = new LogMessage(LogSeverity.Info, name, msg);
            return LogInternal(logMessage);
        }

        public static Task Log(LogMessage logMessage)
        {
            return LogInternal(logMessage);
        }
    }
}
