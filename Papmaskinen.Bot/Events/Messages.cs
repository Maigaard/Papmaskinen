using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.Extensions;

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

				var match = Regex.Match(message.Content, @"^Game: ([\w ]+)");
				if (message.Reference.MessageId.IsSpecified && match != null && match.Success)
				{
					IMessage originalMessage = await message.Channel.GetMessageAsync(message.Reference.MessageId.Value);
					if (originalMessage is IUserMessage userMessage && userMessage.Author.IsBot && userMessage.IsPinned)
					{
						await userMessage.ModifyAsync(prop => prop.EditContent("- Primary game", userMessage.Content, match.Groups[1].Value));
					}
				}
			}
		}
	}
}
