using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Services;

IHost host = Host.CreateDefaultBuilder(args)
				.ConfigureServices(ConfigureServices)
				.Build();
var config = host.Services.GetService<IOptionsSnapshot<DiscordSettings>>();
DiscordSocketClient client = new();

client.Log += (LogMessage msg) =>
{
	Console.WriteLine(msg.ToString());
	return Task.CompletedTask;
};

var token = config?.Value.PublicKey;

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await host.RunAsync();

void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
	services.Configure<DiscordSettings>(a => context.Configuration.Bind("Discord", a));
}
