using Azure.Core;
using Azure.Identity;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Services;

IHost host = Host.CreateDefaultBuilder(args)
		.ConfigureAppConfiguration(ConfigureAppConfiguration)
		.ConfigureServices(ConfigureServices)
		.Build();
var config = host.Services.GetService<IOptionsSnapshot<DiscordSettings>>();

DiscordSocketConfig socketConfig = new()
{
	GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
};
DiscordSocketClient client = new(socketConfig);

client.Log += (LogMessage msg) =>
{
	Console.WriteLine(msg.ToString());
	return Task.CompletedTask;
};

client.MessageReceived += async (SocketMessage message) =>
{
	if (message is SocketUserMessage)
	{
		if (string.Equals(message.Content, "hello", StringComparison.OrdinalIgnoreCase))
		{
			await message.Channel.SendMessageAsync(message.Author.Username);
		}
	}
};

await client.LoginAsync(TokenType.Bot, config?.Value.BotToken);
await client.StartAsync();

await host.RunAsync();
void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
	services.Configure<DiscordSettings>(options => context.Configuration.Bind("Discord", options));
}

void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
{
	var configurationUri = new Uri("https://appc-discord-papmaskinen.azconfig.io");
	builder.AddAzureAppConfiguration(options =>
	{
		TokenCredential tokenCredential = new DefaultAzureCredential();

		options.Connect(configurationUri, tokenCredential);

		options.Select(KeyFilter.Any);
		options.Select(KeyFilter.Any, "Production");
	});
}