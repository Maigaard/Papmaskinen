using System.Text.RegularExpressions;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.Extensions;

internal static class UserExtensions
{
	private static readonly Regex Expression = new(string.Join<IEmote>('|', Emotes.Clocks));

	public static async Task UpdateNickName(this SocketGuildUser user, string? prefix = null, string? suffix = null)
	{
		try
		{
			string cleanedDisplayName = Expression.Replace(user.DisplayName, string.Empty);
			string nickname = $"{prefix}{cleanedDisplayName}{suffix}";
			await user.ModifyAsync(u => u.Nickname = nickname);
		}

		// Catching Discord HttpException when Bot doens't have permission to edit superior user.
		catch (HttpException e) when (e.Message.Contains("error 50013"))
		{
		}
	}
}
