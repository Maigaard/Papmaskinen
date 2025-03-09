using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Papmaskinen.Bot.Events;
using Papmaskinen.Bot.HostedServices;
using Papmaskinen.Integrations.BoardGameGeek.Configuration;

namespace Papmaskinen.Bot.Setup;

internal static class Configurations
{
	internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
	{
		services.Configure<DiscordSettings>(options => context.Configuration.Bind("Discord", options));
		services.AddSingleton<DiscordSocketClient>(_ =>
		{
			DiscordSocketConfig socketConfig = new()
			{
				// Bot permissions: 277226859584
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
		services.AddSingleton<Ready>();
		services.AddSingleton<SubmittedModals>();
		services.AddSingleton<Messages>();
		services.AddSingleton<SlashCommands>();

		services.AddSingleton<ConfigureSocketClient>();
		services.AddSingleton<NextEvent>();
		services.AddHostedService(p => p.GetRequiredService<NextEvent>());
	}
}
