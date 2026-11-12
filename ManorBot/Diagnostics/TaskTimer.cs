using ManorBot.Handlers;
using System.Diagnostics;

namespace ManorBot.Diagnostics
{
    /// <summary>
    /// Handles timing measurements for various bot tasks.
    /// This class prevents violation of the single-responsibility principle
    /// by centralizing performance profiling instead of having each class handle timing independently.
    /// </summary>
    /// <remarks>
    /// See Diagrams/Stopwatch_abstraction.png for a detailed overview of this class's relationships and design.
    /// </remarks>
    internal class TaskTimer(Logger logger)
    {
        /// <summary>
        /// The timestamp when the current task started, in high-resolution ticks.
        /// </summary>
        private long _startTime = 0;

        /// <summary>
        /// Marks the start of a task timer measurement.
        /// </summary>
        public void Start()
        {
            _startTime = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Records the elapsed time since <see cref="Start"/> was called and logs the result.
        /// Automatically formats the output as milliseconds for times greater than 999 microseconds,
        /// otherwise displays in microseconds.
        /// </summary>
        /// <param name="workDescription">a description of the work completed, used in the log message.
        /// The description should ideally be a gerund verb followed (optionally) by a noun. For example;
        /// "doing something" or "processing".</param>
        /// <returns>A task representing the asynchronous logging operation.</returns>
        public async Task Stop(string workDescription)
        {
            TimeSpan elapsedTime = Stopwatch.GetElapsedTime(_startTime);
            long time = (long)elapsedTime.TotalMicroseconds;

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
