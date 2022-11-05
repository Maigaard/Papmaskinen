using Discord;
using Discord.WebSocket;

namespace Papmaskinen.Bot.Events
{
	public class SlashCommands
	{
		internal async Task SlashCommandReceived(SocketSlashCommand command)
		{
			var task = command.Data.Name switch
			{
				"nominate" => ExecuteNominateCommand(command),
				_ => command.RespondAsync("Unknown command!")
			};

			await task;
		}

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
}
