using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace ManorBot.Handlers
{
    /// <summary>
    /// This class handles any Discord interactions,
    /// primarily slash commands. 
    /// </summary>
    internal class InteractionHandler
    {
        private readonly DiscordSocketClient _cli;
        private readonly ulong _guildId;

        private readonly InteractionService _interactions;

        /// <summary>
        /// The constructor handles initialization of the
        /// InteractionService object. Having InteractionService
        /// be owned by this class rather than the top-level
        /// is a reasonable design decision because it's not 
        /// needed anywhere outside the interaction handling logic.
        /// The top-level code file can be cleaned up this way. 
        /// </summary>
        /// <param name="cli">the client object</param>
        /// <param name="guildId">the ID of the guild where the
        /// slash commands are going to be registered OR 0 if intending
        /// to register them globally instead (recommended for prod)</param>
        public InteractionHandler(DiscordSocketClient cli, ulong guildId)
        {
            _cli = cli;
            _guildId = guildId;
            _interactions = new(_cli);

            _interactions.Log += Logger.Log;
            Task.Run(async () => await SetupAsync()); 
        }   

        private async Task SetupAsync()
        {
            await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        /// <summary>
        /// This method should be only be called by the client.Ready event;
        /// <code>
        /// client.Ready += thisObject.ReadyHandlerAsync;
        /// </code>
        /// Calling this method from anywhere else is untested.
        /// </summary>
        public async Task ReadyHandlerAsync()
        {
            if (_guildId > 0)
                await _interactions.RegisterCommandsToGuildAsync(_guildId);
            else
                await _interactions.RegisterCommandsGloballyAsync();
        }

        /// <summary>
        /// This method should be only be called by the client.Ready event;
        /// <code>
        /// client.InteractionCreated += thisObject.HandleAsync;
        /// </code>
        /// Calling this method from anywhere else is untested.
        /// </summary>
        public async Task HandleAsync(SocketInteraction interaction)
        {
            var context = new SocketInteractionContext(_cli, interaction);
            // TODO: May wanna check context properties before ExecuteCommandAsync
            await _interactions.ExecuteCommandAsync(context, null);
        }
    }
}
