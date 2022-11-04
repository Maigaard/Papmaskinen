using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Integrations.BoardGameGeek;
using Papmaskinen.Integrations.BoardGameGeek.Models;

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
                    await message.Channel.SendMessageAsync($"Hi {message.Author.Username}");
                }

                if (message.Content.StartsWith("Game:"))
                {
                    bool parsed = int.TryParse(message.Content.Substring(6), out int gameId);
                    string gameText = "Couldn't find a game";
                    if (parsed && await this.bggService.GetBoardGame(gameId) is Item game)
                    {
						string gameName = game.Name?.FirstOrDefault(p => p.Type == "primary")?.Value;
						string rating = game.Statistics?.Ratings?.Average?.Value;
						string votes = game?.Statistics?.Ratings?.Usersrated?.Value;
                        gameText = $"Found a game called {gameName}, with an average rating of {rating} on {votes} votes";
                    }

                    await message.Channel.SendMessageAsync(gameText);
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
