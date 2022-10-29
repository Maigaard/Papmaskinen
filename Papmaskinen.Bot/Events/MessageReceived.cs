using Discord.WebSocket;

namespace Papmaskinen.Bot.Events
{
	internal static class MessageReceived
	{
		internal static async Task HelloMessage(SocketMessage message)
		{
			if (message is SocketUserMessage)
			{
				if (string.Equals(message.Content, "hello2", StringComparison.OrdinalIgnoreCase))
				{
					await message.Channel.SendMessageAsync(message.Author.Username);
				}
			}
		}
	}
}
