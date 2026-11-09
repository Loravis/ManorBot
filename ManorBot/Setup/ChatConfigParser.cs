using ManorBot.Handlers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ManorBot.Setup
{
    /// <summary>
    /// This class parses the chat configuration YAML file. 
    /// </summary>
    internal class ChatConfigParser
    {
        /// <summary>
        /// Parses a YAML file and extracts keyword-response pairs into a dictionary.
        /// </summary>
        /// <returns>
        /// A Dictionary&lt;string, string&gt; mapping keywords to their corresponding responses.
        /// Returns an empty dictionary if the file does not exist or contains no keywords.
        /// </returns>
        /// <exception cref="Exception">Thrown if the YAML file cannot be parsed or read.</exception>
        public static Dictionary<string, string> ParseKeywords(Logger logger)
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            filePath = Path.Combine(filePath, "ManorBot", "chat.yaml");

            if (!File.Exists(filePath))
            {
                logger.Log("Chat configuration file not found.");
                return [];
            }

            var yaml = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string content = File.ReadAllText(filePath);
            var config = yaml.Deserialize<YamlConfig>(content);

            var dictionary = new Dictionary<string, string>();

            if (config?.Keywords != null)
            {
                foreach (var keyword in config.Keywords)
                {
                    if (!string.IsNullOrWhiteSpace(keyword.Keyword))
                        dictionary[keyword.Keyword] = keyword.Response ?? string.Empty;
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Internal class representing the structure of the YAML configuration.
        /// </summary>
        private class YamlConfig
        {
            public List<KeywordEntry> Keywords { get; set; } = new();
        }

        /// <summary>
        /// Internal class representing a single keyword-response entry.
        /// </summary>
        private class KeywordEntry
        {
            public string Keyword { get; set; } = string.Empty;
            public string Response { get; set; } = string.Empty;
        }
    }
}
