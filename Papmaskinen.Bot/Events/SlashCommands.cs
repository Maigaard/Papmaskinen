using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.HostedServices;
using Papmaskinen.Bot.Models.Attributes;

namespace Papmaskinen.Bot.Events;

public class SlashCommands(NextEvent nextEventHandler)
{
	private const string NominateCommandName = "nominate";
	private const string ForceNextEventCommandName = "forcenextevent";

	internal async Task SlashCommandReceived(SocketSlashCommand command)
	{
		var task = command.Data.Name switch
		{
			NominateCommandName => ExecuteNominateCommand(command),
			ForceNextEventCommandName => this.ExecuteForceNextEventCommand(command),
			_ => command.RespondAsync("Unknown command!"),
		};

		await task;
	}

	[CommandInfo(NominateCommandName, "Add a new game nomination to the nomination channel.")]
	public static async Task ExecuteNominateCommand(IDiscordInteraction command)
	{
		var modalBuilder = new ModalBuilder()
			.WithCustomId("nomination-modal")
			.WithTitle("Nominate new game")
			.AddTextInput("Board game geek link", "bgg-link");
		var modal = modalBuilder.Build();

		await command.RespondWithModalAsync(modal);
	}

	[CommandInfo(ForceNextEventCommandName, "Force a Next Event message, in case Papmaskinen forgot.")]
	public async Task ExecuteForceNextEventCommand(IDiscordInteraction command)
	{
		nextEventHandler.SetTimer(TimeSpan.FromSeconds(1), cancellationToken: CancellationToken.None);
		await command.RespondAsync("Force next event message has been initiated", ephemeral: true);
	}
}