using Azure.Core;
using Azure.Identity;
using Discord;
using Discord.WebSocket;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Papmaskinen.Bot.Events;
using Papmaskinen.Integrations.BoardGameGeek.Configuration;

namespace Papmaskinen.Bot.Setup
{
	internal static class Configurations
	{
		internal static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
		{
			var configurationUri = new Uri("https://appcs-papmaskinen.azconfig.io");
			builder.AddEnvironmentVariables();
			builder.AddAzureAppConfiguration(options =>
			{
				TokenCredential tokenCredential = new DefaultAzureCredential();

				options.Connect(configurationUri, tokenCredential);
				options.Select(KeyFilter.Any);
				options.Select(KeyFilter.Any, context.HostingEnvironment.EnvironmentName);
			});
		}

		internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
		{
			services.Configure<DiscordSettings>(options => context.Configuration.Bind("Discord", options));
			services.AddSingleton<DiscordSocketClient>(options =>
			{
				DiscordSocketConfig socketConfig = new()
				{
					GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildEmojis | GatewayIntents.GuildMessages | GatewayIntents.GuildMessageReactions | GatewayIntents.MessageContent,
				};
				return new(socketConfig);
			});

			services.AddBoardGameGeek(options => context.Configuration.Bind("BoardGameGeek", options));

			services.AddSingleton<Reactions>();
			services.AddSingleton<SlashCommands>();
			services.AddSingleton<Ready>();
			services.AddSingleton<Messages>();

			services.AddSingleton<ConfigureSocketClient>();
		}

		internal static void ConfigureWebJobs(IWebJobsBuilder builder)
		{
			builder.AddAzureStorageCoreServices();
			builder.AddTimers();
		}
	}
}
