using Discord;

namespace Papmaskinen.Bot.Models.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class CommandInfoAttribute(string commandName, string description) : Attribute
{
	public string CommandName { get; } = commandName;

	public string Description { get; } = description;

	public SlashCommandProperties BuildProperties() => new SlashCommandBuilder()
		.WithName(this.CommandName)
		.WithDescription(this.Description)
		.Build();
}
