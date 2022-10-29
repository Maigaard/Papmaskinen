using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Setup;

IHost host = Host.CreateDefaultBuilder(args)
		.ConfigureAppConfiguration(Configurations.ConfigureAppConfiguration)
		.ConfigureServices(Configurations.ConfigureServices)
		.ConfigureWebJobs(Configurations.ConfigureWebJobs)
		.Build();

var config = host.Services.GetService<IOptionsMonitor<DiscordSettings>>();
var client = host.Services.GetService<DiscordSocketClient>();

await ConfigureSocketClient.Setup(client!, config!.CurrentValue);

await host.RunAsync();
