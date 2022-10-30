using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Papmaskinen.Bot.Events
{
	public class SlashCommands
	{
		internal async Task NominationCommand(SocketSlashCommand command)
		{
			await command.RespondAsync($"You executed {command.Data.Name}");
		}
	}
}
