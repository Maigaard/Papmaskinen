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

var config = host.Services.GetService<ConfigureSocketClient>();

await config!.Setup();

await host.RunAsync();
