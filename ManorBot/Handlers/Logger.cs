using System.Threading.Channels;
using Discord;

namespace ManorBot.Handlers
{
    /// <summary>
    /// Class to handle unified processing of log messages.
    /// Uses a channel-based async queue to process logs asynchronously.
    /// </summary>
    internal class Logger(string name)
    {
        /// <summary>
        /// Unbounded channel used to queue log messages for asynchronous processing.
        /// Configured for single reader and multiple writers.
        /// </summary>
        private static readonly Channel<LogMessage> Queue =
            Channel.CreateUnbounded<LogMessage>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });

        /// <summary>
        /// The consumer task that processes all queued log messages.
        /// </summary>
        private static readonly Task Consumer;

        static Logger()
        {
            Consumer = Task.Run(ProcessQueueAsync);

            AppDomain.CurrentDomain.ProcessExit += (_, _) => Shutdown();
        }

        /// <summary>
        /// Processes all queued log messages asynchronously, writing them to the console.
        /// Runs until the queue is completed and all messages have been processed.
        /// </summary>
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

        /// <summary>
        /// Gracefully shuts down the logger by completing the queue and waiting for the consumer task to finish.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for the consumer task to complete. Defaults to 5 seconds.</param>
        public static void Shutdown(TimeSpan? timeout = null)
        {
            Queue.Writer.TryComplete();
            Consumer.Wait(timeout ?? TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Logs an informational message with this logger's name as the source.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <returns>A completed task.</returns>
        public Task Log(string msg)
        {
            return Log(new LogMessage(LogSeverity.Info, name, msg));
        }

        /// <summary>
        /// Logs a Discord log message to the queue for asynchronous processing.
        /// </summary>
        /// <param name="logMessage">The Discord log message to process.</param>
        /// <returns>A completed task.</returns>
        public static Task Log(LogMessage logMessage)
        {
            Queue.Writer.TryWrite(logMessage);
            return Task.CompletedTask;
        }
    }
}
