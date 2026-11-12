using Discord;
using Discord.WebSocket;
using ManorBot.Diagnostics;
using ManorBot.Handlers;
using ManorBot.Setup;

/* TODO: Make the bot serve waffles
 * TODO: Document this file
 */

EnvParse.Parse();
var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
var botName = Environment.GetEnvironmentVariable("BOT_NAME");
botName ??= "Bot";
bool res = UInt64.TryParse(Environment.GetEnvironmentVariable("BOT_GUILD_ID"), out ulong guildId);

var botLogger = new Logger(botName);
var timer = new TaskTimer(botLogger);

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

var inHandler = new InteractionHandler(client, guildId);
var msgHandler = new MsgHandler(timer, botLogger);

client.Log += Logger.Log;
client.MessageReceived += msgHandler.HandleAsync;

client.Ready += inHandler.ReadyHandlerAsync;

client.InteractionCreated += inHandler.HandleAsync;

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await Task.Delay(-1);