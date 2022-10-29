using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.Events;

namespace Papmaskinen.Bot.Setup
{
	internal static class ConfigureSocketClient
	{
		internal static async Task Setup(DiscordSocketClient client, DiscordSettings settings)
		{
			client.Log += (LogMessage msg) =>
			{
				Console.WriteLine(msg.ToString());
				return Task.CompletedTask;
			};

			client.MessageReceived += MessageReceived.HelloMessage;

			await client.LoginAsync(TokenType.Bot, settings.BotToken);
			await client.StartAsync();
		}
	}
}
