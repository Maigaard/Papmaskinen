using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Integrations.BoardGameGeek.Models;
using Papmaskinen.Integrations.BoardGameGeek.Services;

namespace Papmaskinen.Bot.Events;

public class Messages(BoardGameGeekService bggService)
{
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
			string gameText = await this.GetGameText(parsed, gameId);

			await userMessage.Channel.SendMessageAsync(gameText);
		}

		var match = Regex.Match(userMessage.Content, @"^Game: ([\w ]+)");
		if (userMessage.Reference is { MessageId.IsSpecified: true } && match.Success)
		{
			IMessage originalMessage = await userMessage.Channel.GetMessageAsync(userMessage.Reference.MessageId.Value);
			if (originalMessage is IUserMessage originalUserMessage && originalUserMessage.Author.IsBot && originalUserMessage.IsPinned)
			{
				await originalUserMessage.ModifyAsync(prop => prop.EditContent("- Primary game", originalUserMessage.Content, match.Groups[1].Value));
			}
		}
	}

	private async Task<string> GetGameText(bool parsed, int gameId)
	{
		if (!parsed || await bggService.GetBoardGame(gameId) is not { } game)
		{
			return "Couldn't find a game";
		}

		string? gameName = game.Name?.Find(p => p.Type == "primary")?.Value;
		string? rating = game.Statistics?.Ratings?.Average?.Value;
		string? votes = game.Statistics?.Ratings?.Usersrated?.Value;
		return $"Found a game called {gameName}, with an average rating of {rating} on {votes} votes";
	}
}