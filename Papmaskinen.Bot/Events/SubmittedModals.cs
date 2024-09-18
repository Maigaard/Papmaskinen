using Discord.WebSocket;
using Papmaskinen.Bot.Models;
using Papmaskinen.Bot.Models.Constants;
using Papmaskinen.Integrations.BoardGameGeek.Services;
using SmartFormat;

namespace Papmaskinen.Bot.Events;

public class SubmittedModals(BoardGameGeekService bggService)
{
	public async Task ModalSubmitted(SocketModal modal)
	{
		var bggLinkComponent = modal.Data.Components.Single(x => x.CustomId == "bgg-link");
		if (!TryGetGameId(bggLinkComponent.Value, out int gameId))
		{
			await modal.RespondAsync($"{bggLinkComponent.Value} is an invalid BoardGameGeek Url", ephemeral: true);
			return;
		}

		var game = await bggService.GetBoardGame(gameId);
		if (game == null)
		{
			await modal.RespondAsync($"Couldn't find a game with id: {gameId}", ephemeral: true);
			return;
		}

		var nomination = new Nomination(game, bggLinkComponent.Value);
		string content = Smart.Format(MessageTemplates.NewNominationTemplate, nomination);
		await modal.RespondAsync(content);
	}

	private static bool TryGetGameId(string gameUrl, out int gameId)
	{
		try
		{
			string pathValue = new Uri(gameUrl).Segments[2].TrimEnd('/');
			return int.TryParse(pathValue, out gameId);
		}
		catch (Exception)
		{
			gameId = 0;
			return false;
		}
	}
}
