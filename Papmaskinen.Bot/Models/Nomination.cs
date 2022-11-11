namespace Papmaskinen.Bot.Models;

public record Nomination
{
	public string? BoardGameGeekUrl { get; set; }

	public string? Name { get; set; }

	public string? Description { get; set; }
}