using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.HostedServices;
using Papmaskinen.Bot.Models.Attributes;

namespace Papmaskinen.Bot.Events;

public class SlashCommands(NextEvent nextEventHandler)
{
	private const string NominateCommandName = "nominate";
	private const string ForceNextEvent = "forceNextEvent";

	internal async Task SlashCommandReceived(SocketSlashCommand command)
	{
		var task = command.Data.Name switch
		{
			NominateCommandName => ExecuteNominateCommand(command),
			ForceNextEvent => this.ExecuteForceNextEventCommand(command),
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

	[CommandInfo(ForceNextEvent, "Force a Next Event message, in case the Papmaskinen forgot.")]
	private async Task ExecuteForceNextEventCommand(IDiscordInteraction command)
	{
		nextEventHandler.SetTimer(TimeSpan.FromSeconds(1), cancellationToken: CancellationToken.None);
		await command.RespondAsync("Force next event message has been initiated", ephemeral: true);
	}
}