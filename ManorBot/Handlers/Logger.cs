using System.Threading.Channels;
using Discord;

namespace ManorBot.Handlers
{
    internal class Logger(string name)
    {
        private static readonly Channel<LogMessage> Queue =
            Channel.CreateUnbounded<LogMessage>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });

        private static readonly Task Consumer;

        static Logger()
        {
            Consumer = Task.Run(ProcessQueueAsync);

            AppDomain.CurrentDomain.ProcessExit += (_, _) => Shutdown();
        }

        private static async Task ProcessQueueAsync()
        {
            await foreach (var log in Queue.Reader.ReadAllAsync())
            {
                try
                {
                    Console.WriteLine(log.ToString());
                }
                catch (Exception ex)
                {
                    try { Console.Error.WriteLine($"Logger failure: {ex}"); } catch { /* ignore */ }
                }
            }
        }

        public static void Shutdown(TimeSpan? timeout = null)
        {
            Queue.Writer.TryComplete();
            Consumer.Wait(timeout ?? TimeSpan.FromSeconds(5));
        }

        public Task Log(string msg)
        {
            return Log(new LogMessage(LogSeverity.Info, name, msg));
        }

        public static Task Log(LogMessage logMessage)
        {
            Queue.Writer.TryWrite(logMessage);
            return Task.CompletedTask;
        }
    }
}
