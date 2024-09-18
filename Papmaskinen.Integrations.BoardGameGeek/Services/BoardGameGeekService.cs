using System.Collections.Specialized;
using System.Web;
using Papmaskinen.Integrations.BoardGameGeek.Models;

namespace Papmaskinen.Integrations.BoardGameGeek.Services;

public class BoardGameGeekService(BoardGameGeekHttpClient client)
{
	public async Task<Item?> GetBoardGame(int boardGameId)
	{
		NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
		query.Add("stats", "1");
		query.Add("id", boardGameId.ToString());

		var result = await client.GetAsync<ThingResultSet>($"thing?{query}");

		return result?.Items?.FirstOrDefault();
	}
}