using System.Globalization;
using Papmaskinen.Integrations.BoardGameGeek.Models;

namespace Papmaskinen.Bot.Models;

public record Nomination
{
	public Nomination(Item bggItem, string link)
	{
		double.TryParse(bggItem.Statistics?.Ratings?.Average?.Value, CultureInfo.InvariantCulture, out double rating);
		int.TryParse(bggItem.Minplayers?.Value, out int minPlayers);
		int.TryParse(bggItem.Maxplayers?.Value, out int maxPlayers);

		this.Link = link;
		this.Name = bggItem.Name?.Find(p => p.Type == "primary")?.Value;
		this.Rating = rating;
		string? limitedDescription = bggItem.Description?[..Math.Min(bggItem.Description.Length, 1400)];
		int? lastIndex = limitedDescription?.LastIndexOf("&#10;&#10;");
		this.Description = lastIndex < 0 ? $"{limitedDescription}..." : limitedDescription![0..lastIndex!.Value];
		this.Description = this.Description.Replace("&#10;&#10;", "\n\n");
		this.Players = minPlayers == maxPlayers ? $"{minPlayers}-{maxPlayers}" : maxPlayers.ToString();
		this.Mechanics = bggItem.Link?.Where(l => l.Type == "boardgamemechanic").Select(l => l.Value!).ToList() ?? Enumerable.Empty<string>();
	}

	public string? Name { get; set; }

	public string Link { get; set; }

	private string? Description { get; set; }

	public string Players { get; set; }

	public double Rating { get; set; }

	public IEnumerable<string> Mechanics { get; set; }
}