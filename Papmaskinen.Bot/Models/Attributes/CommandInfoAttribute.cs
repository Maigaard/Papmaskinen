using Discord;

namespace Papmaskinen.Bot.Models.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class CommandInfoAttribute : Attribute
{
	public CommandInfoAttribute(string commandName, string description)
	{
		this.CommandName = commandName;
		this.Description = description;
	}

	public string CommandName { get; }

	public string Description { get; }

	public SlashCommandProperties BuildProperties() => new SlashCommandBuilder()
		.WithName(this.CommandName)
		.WithDescription(this.Description)
		.Build();
}
