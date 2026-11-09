using System.Text.RegularExpressions;

namespace ManorBot.Setup
{
    /// <summary>
    /// Utility class for parsing environment variables from a .env file in the user's profile directory.
    /// </summary>
    /// <remarks>
    /// This class reads from %UserProfile%/.env if it exists and parses environment variables.
    /// Only string values formatted as VARIABLE="VALUE" are recognized.
    /// Non-string values or incorrectly formatted lines are ignored.
    /// </remarks>
    internal class EnvParse
    {
        /// <summary>
        /// Parses environment variables from a .env file located in the user's home directory (%UserProfile%/.env).
        /// Environment variables must be formatted as follows: VARIABLE="VALUE".
        /// </summary>
        /// <returns>
        /// True if at least one variable was successfully parsed from the .env file.
        /// False if the .env file does not exist.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if the user profile directory (%UserProfile%) cannot be determined.
        /// This indicates an exceptional system state that should not occur in normal operation.
        /// </exception>
        public static bool Parse()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (path == "")
                throw new Exception("A user profile directory could not be found.");

            path = Path.Combine(path, ".env");

            if (!File.Exists(path))
                return false;

            string[] lines = File.ReadAllLines(path);

            bool parsed = false;
            foreach (var line in lines)
            {
                string key;
                string value;

                var match = Regex.Match(line, "^([^=]+)=\"(.*)\"$");

                if (!(match.Success))
                    continue;

                parsed = true;

                key = match.Groups[1].Value;
                value = match.Groups[2].Value;

                Environment.SetEnvironmentVariable(key, value);
            }

            return parsed;
        }
    }
}
