using System.Reflection;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Models.Attributes;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.Events;

public class Ready
{
	private readonly ILogger<Ready> logger;
	private readonly DiscordSocketClient client;
	private readonly DiscordSettings settings;

	public Ready(ILogger<Ready> logger, IOptionsMonitor<DiscordSettings> options, DiscordSocketClient client)
	{
		this.settings = options.CurrentValue;
		this.logger = logger;
		this.client = client;
	}

	internal async Task InstallCommands()
	{
		var guild = this.client.GetGuild(this.settings.GuildId);
		if (guild is null)
		{
			this.logger.LogWarning("Couldn't find guild.");
			return;
		}

		IEnumerable<CommandInfoAttribute> botCommands = GetCommands();
		IReadOnlyCollection<SocketApplicationCommand> applicationCommands = await guild.GetApplicationCommandsAsync();
		if (botCommands.ExceptBy(applicationCommands.Select(bc => bc.Name), ac => ac.CommandName).Any())
		{
			this.logger.LogInformation("Commands already installed.");
			return;
		}

		var deleteTasks = applicationCommands.Select(ac => ac.DeleteAsync());
		await Task.WhenAll(deleteTasks);
		await Task.WhenAll(botCommands.Select(ci => guild.CreateApplicationCommandAsync(ci.BuildProperties())));
	}

	private static IEnumerable<CommandInfoAttribute> GetCommands()
	{
		return typeof(SlashCommands)
			.GetMethods()
			.Select(m => m.GetCustomAttribute<CommandInfoAttribute>())
			.Where(m => m is not null)!;
	}
}
