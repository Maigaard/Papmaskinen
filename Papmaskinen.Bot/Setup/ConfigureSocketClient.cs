using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Events;

namespace Papmaskinen.Bot.Setup
{
	public class ConfigureSocketClient
	{
		private readonly DiscordSocketClient client;
		private readonly Reactions reactions;
		private readonly DiscordSettings settings;

		public ConfigureSocketClient(DiscordSocketClient client, IOptionsMonitor<DiscordSettings> settings, Reactions reactions)
		{
			this.client = client;
			this.reactions = reactions;
			this.settings = settings.CurrentValue;
		}

		internal async Task Setup()
		{
			this.client.Log += (LogMessage msg) =>
			{
				Console.WriteLine(msg.ToString());
				return Task.CompletedTask;
			};

			this.client.MessageReceived += Messages.MessageReceived;
			this.client.ReactionAdded += this.reactions.Added;
			this.client.ReactionRemoved += this.reactions.Removed;

			await this.client.LoginAsync(TokenType.Bot, this.settings.BotToken);
			await this.client.StartAsync();
		}
	}
}
