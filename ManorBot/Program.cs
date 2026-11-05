using Discord;
using Discord.WebSocket;
using ManorBot.Handlers;
using ManorBot.Setup;

/* TODO: Make the bot serve waffles */

EnvParse.Parse();
var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
var botName = Environment.GetEnvironmentVariable("BOT_NAME");
botName ??= "Bot";

var botLogger = new Logger(botName);

if (token == null)
{
    await Logger.Log(new LogMessage(LogSeverity.Error, botName, "Setup Error: BOT_TOKEN is not set!"));
    Environment.Exit(1);
}

var config = new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
};

var client = new DiscordSocketClient(config);
var msgHandler = new MsgHandler();
client.Log += Logger.Log;
client.MessageReceived += msgHandler.HandleAsync;

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await Task.Delay(-1);