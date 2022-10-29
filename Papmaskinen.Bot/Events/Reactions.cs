using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.Events
{
	public class Reactions
	{
		private readonly DiscordSettings settings;
		private readonly DiscordSocketClient client;

		public Reactions(DiscordSocketClient client, IOptionsMonitor<DiscordSettings> options)
		{
			this.settings = options.CurrentValue;
			this.client = client;
		}

		internal async Task NextEventReactions(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
		{
			IUserMessage? message = (messageCache.Value ?? await reaction.Channel.GetMessageAsync(messageCache.Id)) as IUserMessage;
			if (ShouldReact(reaction, message))
			{
				await ReactToEmote(reaction, message!, Emotes.ThumbsUp, "Attending");
				await ReactToEmote(reaction, message!, Emotes.FingersCrossed, "Hopefully");
				await ReactToEmote(reaction, message!, Emotes.House, "- Place");
				await ReactToEmote(reaction, message!, Emotes.GameDie, "- Game Master");
			}
		}

		private static bool ShouldReact(SocketReaction reaction, IUserMessage? message)
		{
			return reaction.User.GetValueOrDefault()?.IsBot == false
				 && message is not null && message.Author.IsBot;
		}

		private static async Task ReactToEmote(SocketReaction reaction, IUserMessage message, IEmote emote, string messagePrefix)
		{
			if (reaction.Emote.Name == emote.Name)
			{
				string usernames = await GetUserNames(message, emote);
				await message!.ModifyAsync((prop) =>
				{
					prop.Content = EditMessage(messagePrefix, message.Content, usernames);
				});
			}
		}

		private static async Task<string> GetUserNames(IUserMessage message, IEmote emote)
		{
			var users = await message.GetReactionUsersAsync(emote, 20).FlattenAsync();
			var userNames = users.Where(u => !u.IsBot).Select(u => u is IGuildUser user ? user.Nickname : u.Username);
			return userNames.Any() ? string.Join(", ", userNames) : "TBD";
		}

		private static string EditMessage(string prefix, string input, string replacement)
		{
			string pattern = $@"({prefix}:(\s|\r\n))([\w, ]*)(\r\n)";
			return Regex.Replace(input, pattern, $"$1{replacement}$4");
		}
	}
}
