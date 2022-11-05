using Discord;
using Discord.WebSocket;

namespace Papmaskinen.Bot.Events
{
	public class SlashCommands
	{
		internal async Task SlashCommandReceived(SocketSlashCommand command)
		{
			switch (command.Data.Name)
			{
				case "nominate":
					await ExecuteNominateCommand(command);
					break;
				default:
					await command.RespondAsync("Unknown command!");
					break;
			}
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
