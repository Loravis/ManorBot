using System.Text.RegularExpressions;

namespace ManorBot.Setup
{
    internal class EnvParse
    { 
        /* This method parses environment variables found in %UserProfile%/.env,
         * if that file exists.
         * 
         * Returns: True if variables have been parsed, False if otherwise. 
         * Throws:  Exception if %UserProfile% does not exist. 
         * 
         * Only string values can be parsed, non-string values are ignored.
         * String values must be formatted as follows: VARIABLE="VALUE".
         * 
         * It is appropriate to raise an exception in the scenario 
         * of %UserProfile% not existing, because this would be
         * an exceptional case that shouldn't occur often and 
         * usually indicates that something is wrong with the 
         * host system (or the code). */
        public static bool Parse()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (path == "")
                throw new Exception("A user profile directory could not be found.");

            path = Path.Combine(path, ".env");

            string[] lines;
            try
            {
                lines = File.ReadAllLines(path);
            } catch (FileNotFoundException)
            {
                return false;
            }

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
