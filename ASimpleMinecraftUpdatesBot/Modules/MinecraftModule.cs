using ASimpleMinecraftUpdatesBot.Services;
using Discord.Interactions;
using Discord.WebSocket;

namespace ASimpleMinecraftUpdatesBot.Modules
{
    public class MinecraftModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly JsonService _jsonService;
        private readonly MinecraftService _mcService;
        private readonly ConfigService _configService;


        public MinecraftModule(JsonService jsonService, MinecraftService mcService, ConfigService configService)
        {
            _jsonService = jsonService;
            _mcService = mcService;
            _configService = configService;
        }

        [SlashCommand("status", "Get the current status of the Minecraft server.")]
        public async Task StatusCommandAsync()
        {
            await DeferAsync();
            var config = _jsonService.Config;
            var status = await _mcService.GetFullStatus(config);
            if (status.IsOnline)
            {
                await FollowupAsync($"✅ **Server Online!** Players: `{status.CurrentOverMaxPlayers}`");
            }
            else
            {
                await FollowupAsync("❌ **Server Offline.** Check the IP or port settings.");
            }
        }

        [SlashCommand("setup", "Configure the Minecraft server settings.")]
        public async Task SetupCommandAsync(
            [Summary("name", "The name of the server")] string serverName,
            [Summary("ip", "The IP address of the server")] string ip,
            [Summary("port", "The port (default is 25565)")] ushort port = 25565,
            [Summary("channel", "The channel for updates")] SocketTextChannel? channel = null)
        {
            await DeferAsync(ephemeral: true);
            try
            {
                _configService.UpdateConfig(serverName, ip, port, channel?.Id);
                await FollowupAsync($"✅ **Settings Saved!**\nIP: `{ip}`\nPort: `{port}`\nChannel: {channel?.Mention ?? "None"}");
            }
            catch (Exception ex)
            {
                await FollowupAsync($"❌ **Error saving config:** {ex.Message}");
            }
        }

        [SlashCommand("pinghost", "Check if the physical server is reachable.")]
        public async Task PingHostAsync()
        {
            await DeferAsync();
            //ping the ip associated with it
            bool isAlive = await _mcService.PingComputerAsync(_jsonService.Config.MinecraftIp);
            await FollowupAsync(isAlive ? "🖥️ Host is reachable!" : "💀 Host is unreachable (Offline or Firewall blocking).");
        }
    }
}