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

			var builder = new SlashCommandBuilder();
			builder.WithName("nominate");
			builder.WithDescription("Add a new nominations to the nomination channel");
			builder.AddOptions(
				new SlashCommandOptionBuilder
				{
					Type = ApplicationCommandOptionType.String,
					Name = "boardgamegeek-link",
					Description = "Link to the boardgame you wish to nominate",
					IsRequired = true,
					IsDefault = false,
				},
				new SlashCommandOptionBuilder
				{
					Type = ApplicationCommandOptionType.String,
					Name = "additional-description",
					Description = "If you have something else to add that isn't in the Boardgamegeek description.",
					IsRequired = false,
					IsDefault = false,
					MaxLength = 100,
				});
			await guild.CreateApplicationCommandAsync(builder.Build());
		}
	}
}
