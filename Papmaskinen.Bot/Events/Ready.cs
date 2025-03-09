using System.Reflection;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Models.Attributes;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.Events;

public class Ready(ILogger<Ready> logger, IOptionsMonitor<DiscordSettings> options, DiscordSocketClient client)
{
	private readonly DiscordSettings settings = options.CurrentValue;

	internal async Task InstallCommands()
	{
		var guild = client.GetGuild(this.settings.GuildId);
		if (guild is null)
		{
			logger.LogWarning("Couldn't find guild.");
			return;
		}

		IEnumerable<CommandInfoAttribute> botCommands = GetCommands().ToList();
		IReadOnlyCollection<SocketApplicationCommand> applicationCommands = await guild.GetApplicationCommandsAsync();
		if (botCommands.ExceptBy(applicationCommands.Select(bc => bc.Name), ac => ac.CommandName).Any() == false)
		{
			logger.LogInformation("Commands already installed.");
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
