using Discord;
using Discord.WebSocket;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Services;

namespace Papmaskinen.Bot
{
	public class Function
	{
		private readonly IOptionsSnapshot<DiscordSettings> settings;

		public Function(IOptionsSnapshot<DiscordSettings> settings)
		{
			this.settings = settings;
		}

		public async Task CronFunction([TimerTrigger("%Discord:NextEvent:Schedule%")] TimerInfo timer)
		{
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

			await client.LoginAsync(TokenType.Bot, this.settings.Value.BotToken);
			await client.StartAsync();

			var channel = await client.GetChannelAsync(this.settings.Value.NextEvent.ChannelId);
			if (channel is ITextChannel ch)
			{
				await ch.SendMessageAsync(DateTime.Now.ToString());
			}

			await client.StopAsync();
		}
	}
}
