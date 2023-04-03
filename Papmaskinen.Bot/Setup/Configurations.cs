using Azure.Core;
using Azure.Identity;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Papmaskinen.Bot.Events;
using Papmaskinen.Bot.HostedServices;
using Papmaskinen.Integrations.BoardGameGeek.Configuration;

namespace Papmaskinen.Bot.Setup;

internal static class Configurations
{
	internal static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
	{
		if (Environment.GetEnvironmentVariable("APP_CONFIG_STORE") is string appConfigStore)
		{
			var configurationUri = new Uri(appConfigStore);
			builder.AddEnvironmentVariables();
			builder.AddAzureAppConfiguration(options =>
			{
				TokenCredential tokenCredential = new DefaultAzureCredential();
				options.Connect(configurationUri, tokenCredential);
				options.Select(KeyFilter.Any);
				options.Select(KeyFilter.Any, context.HostingEnvironment.EnvironmentName);
			});
		}
	}

	internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
	{
		services.Configure<DiscordSettings>(options => context.Configuration.Bind("Discord", options));
		services.AddSingleton<DiscordSocketClient>(options =>
		{
			DiscordSocketConfig socketConfig = new()
			{
				// Bot permissions: 1376738487360
				GatewayIntents =
					GatewayIntents.Guilds |
					GatewayIntents.GuildEmojis |
					GatewayIntents.GuildMessages |
					GatewayIntents.GuildMessageReactions |
					GatewayIntents.MessageContent |
					GatewayIntents.GuildMembers,
			};
			return new(socketConfig);
		});

		services.AddBoardGameGeek(options => context.Configuration.Bind("BoardGameGeek", options));

		services.AddSingleton<Reactions>();
		services.AddSingleton<SlashCommands>();
		services.AddSingleton<Ready>();
		services.AddSingleton<SubmittedModals>();
		services.AddSingleton<Messages>();

		services.AddSingleton<ConfigureSocketClient>();
		services.AddHostedService<NextEvent>();
	}
}
