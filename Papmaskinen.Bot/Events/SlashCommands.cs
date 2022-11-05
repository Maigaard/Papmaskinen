using Discord;
using Discord.WebSocket;

namespace Papmaskinen.Bot.Events
{
	public class SlashCommands
	{
		internal async Task NominationCommand(SocketSlashCommand command)
		{
			if (command.Data.Name == "nominate")
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
}
