using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.Events
{
	public class Ready
	{
		private readonly DiscordSocketClient client;
		private readonly DiscordSettings settings;

		public Ready(IOptionsMonitor<DiscordSettings> options, DiscordSocketClient client)
		{
			this.settings = options.CurrentValue;
			this.client = client;
		}

		internal async Task InstallCommands()
		{
			var guild = this.client.GetGuild(this.settings.GuildId);
			if (guild == null || (await guild.GetApplicationCommandsAsync()).Any(ac => ac.Name == "nominate"))
			{
				Console.WriteLine("Skipping");
				return;
			}

			var deleteTasks = (await guild.GetApplicationCommandsAsync()).Select(ac => ac.DeleteAsync());
			await Task.WhenAll(deleteTasks);

			var builder = new SlashCommandBuilder();
			builder.WithName("nominate");
			builder.WithDescription("Add a new nominations to the nomination channel");

			await guild.CreateApplicationCommandAsync(builder.Build());
		}
	}
}
