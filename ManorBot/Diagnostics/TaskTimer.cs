using ManorBot.Handlers;
using System.Diagnostics;

namespace ManorBot.Diagnostics
{
    /* This class handles getting the time 
     * it takes to complete various tasks. 
     * It prevents violating the single-responsibility
     * principle by having each class handle time 
     * stopping on its own. 
     * 
     * See Diagrams/Stopwatch_abstraction.png for 
     * a rundown of this class' relationships */
    internal class TaskTimer(Logger logger)
    {
        long startTime = 0;

        public void Start()
        {
            startTime = Stopwatch.GetTimestamp();
        }

        public async Task Stop(string workDescription)
        {
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
