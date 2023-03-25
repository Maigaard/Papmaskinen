using System.Collections.Specialized;
using System.Web;
using Papmaskinen.Integrations.BoardGameGeek.Models;

namespace Papmaskinen.Integrations.BoardGameGeek.Services;

public class BoardGameGeekService
{
	private readonly BoardGameGeekHttpClient client;

	public BoardGameGeekService(BoardGameGeekHttpClient client)
	{
		this.client = client;
	}

	public async Task<Item?> GetBoardGame(int boardGameId)
	{
		NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
		query.Add("stats", "1");
		query.Add("id", boardGameId.ToString());

		var result = await this.client.GetAsync<ThingResultSet>($"thing?{query}");

		return result?.Items?.FirstOrDefault();
	}
}