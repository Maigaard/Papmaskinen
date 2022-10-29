using Discord.WebSocket;

namespace Papmaskinen.Bot.Events
{
	internal static class Messages
	{
		internal static async Task MessageReceived(SocketMessage message)
		{
			if (message is SocketUserMessage)
			{
				if (string.Equals(message.Content, "Hi bot", StringComparison.OrdinalIgnoreCase))
				{
					await message.Channel.SendMessageAsync($"Hi {message.Author.Username}");
				}
			}
		}
	}
}
