using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.Models.Attributes;

namespace Papmaskinen.Bot.Events;

internal static class SlashCommands
{
	private const string NominateCommandName = "nominate";

	internal static async Task SlashCommandReceived(SocketSlashCommand command)
	{
		var task = command.Data.Name switch
		{
			NominateCommandName => ExecuteNominateCommand(command),
			_ => command.RespondAsync("Unknown command!"),
		};

		await task;
	}

	[CommandInfo(NominateCommandName, "Add a new game nomination to the nomination channel.")]
	private static async Task ExecuteNominateCommand(IDiscordInteraction command)
	{
		var modalBuilder = new ModalBuilder()
			.WithCustomId("nomination-modal")
			.WithTitle("Nominate new game")
			.AddTextInput("Board game geek link", "bgg-link");
		var modal = modalBuilder.Build();

		await command.RespondWithModalAsync(modal);
	}
}