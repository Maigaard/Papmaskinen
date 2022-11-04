using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Integrations.BoardGameGeek;

namespace Papmaskinen.Bot.Events
{
	public class Messages
	{
		private readonly BoardGameGeekService bggService;

		public Messages(BoardGameGeekService bggService)
		{
			this.bggService = bggService;
		}

		internal async Task MessageReceived(SocketMessage message)
		{
			if (message is SocketUserMessage)
			{
				if (string.Equals(message.Content, "Hi bot", StringComparison.OrdinalIgnoreCase))
				{
					string name = await this.bggService.GetBoardGame(328636);
					Console.WriteLine(name);
					await message.Channel.SendMessageAsync($"Hi {message.Author.Username}");
				}

				var match = Regex.Match(message.Content, @"^Game: ([\w ]+)");
				if (message.Reference != null && message.Reference.MessageId.IsSpecified && match != null && match.Success)
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
