using Discord;
using Discord.WebSocket;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot
{
	public class Function
	{
		private readonly DiscordSettings settings;
		private readonly DiscordSocketClient client;

		public Function(IOptionsMonitor<DiscordSettings> settings, DiscordSocketClient client)
		{
			this.settings = settings.CurrentValue;
			this.client = client;
		}

		public async Task CronFunction([TimerTrigger("%Discord:NextEvent:Schedule%")] TimerInfo timer)
		{
			var channel = await this.client.GetChannelAsync(this.settings.NextEvent.ChannelId);
			if (channel is ITextChannel ch)
			{
				await ch.SendMessageAsync(DateTime.Now.ToString());
			}
		}
	}
}
