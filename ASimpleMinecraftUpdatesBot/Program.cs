using ASimpleMinecraftUpdatesBot.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMessages,
    AlwaysDownloadUsers = true
});

builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton(provider =>
{
    var client = provider.GetRequiredService<DiscordSocketClient>();
    return new InteractionService(client);
});
builder.Services.AddSingleton<JsonService>();
builder.Services.AddSingleton<ConfigService>();
builder.Services.AddSingleton<MinecraftService>();

using IHost host = builder.Build();

var client = host.Services.GetRequiredService<DiscordSocketClient>();
var interactionService = host.Services.GetRequiredService<InteractionService>();

client.Ready += async () =>
{
    await interactionService.AddModulesAsync(typeof(Program).Assembly, host.Services);
    await Task.Delay(3000);
    var guilds = client.Guilds;
    if (guilds != null)
    {
        await interactionService.AddModulesAsync(typeof(Program).Assembly, host.Services);
        await interactionService.RegisterCommandsToGuildAsync(guild.Id);
        foreach (SocketGuild guild in guilds)
        {
            
            await interactionService.RegisterCommandsToGuildAsync(guild.Id);

            Console.WriteLine($"✅ Registered {interactionService.Modules.Count()} modules to {guild.Name}");
            Console.WriteLine($"✅ Connected to {guild.Name} ({guild.Id}) and registered commands!");
        }
    }
    else
    {
        Console.WriteLine("⚠️ The bot isn't in any servers yet! Invite it to your server first.");
    }
    Console.WriteLine($"✅ Connected all guilds! {client.Guilds.Count}");
};

client.SlashCommandExecuted += async (interaction) =>
{
    var ctx = new SocketInteractionContext(client, interaction);
    await interactionService.ExecuteCommandAsync(ctx, host.Services);
};

// Login and Start
await client.LoginAsync(Discord.TokenType.Bot, "YOUR_BOT_TOKEN");
await client.StartAsync();

await host.RunAsync();