using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Integrations.BoardGameGeek.Models;
using Papmaskinen.Integrations.BoardGameGeek.Services;

namespace Papmaskinen.Bot.Events;

public class Messages
{
	private readonly BoardGameGeekService bggService;

	public Messages(BoardGameGeekService bggService)
	{
		this.bggService = bggService;
	}

	internal async Task MessageReceived(SocketMessage message)
	{
		if (message is not SocketUserMessage userMessage)
		{
			return;
		}

		if (string.Equals(userMessage.Content, "Hi bot", StringComparison.OrdinalIgnoreCase))
		{
			await userMessage.Channel.SendMessageAsync($"Hi {userMessage.Author.Username}");
		}

		if (userMessage.Content.StartsWith("Game:"))
		{
			bool parsed = int.TryParse(userMessage.Content[6..], out int gameId);
			string gameText = "Couldn't find a game";

			if (parsed && await this.bggService.GetBoardGame(gameId) is Item game)
			{
				string? gameName = game.Name?.FirstOrDefault(p => p.Type == "primary")?.Value;
				string? rating = game.Statistics?.Ratings?.Average?.Value;
				string? votes = game.Statistics?.Ratings?.Usersrated?.Value;
				gameText = $"Found a game called {gameName}, with an average rating of {rating} on {votes} votes";
			}

			await userMessage.Channel.SendMessageAsync(gameText);
		}

		var match = Regex.Match(userMessage.Content, @"^Game: ([\w ]+)");
		if (userMessage.Reference != null && userMessage.Reference.MessageId.IsSpecified && match != null && match.Success)
		{
			IMessage originalMessage = await userMessage.Channel.GetMessageAsync(userMessage.Reference.MessageId.Value);
			if (originalMessage is IUserMessage originalUserMessage && originalUserMessage.Author.IsBot && originalUserMessage.IsPinned)
			{
				await originalUserMessage.ModifyAsync(prop => prop.EditContent("- Primary game", originalUserMessage.Content, match.Groups[1].Value));
			}
		}
	}
}
