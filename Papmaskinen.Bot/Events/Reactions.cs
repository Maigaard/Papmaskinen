using Discord;
using Discord.WebSocket;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.Events
{
	public class Reactions
	{
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
				 && message is not null && message.Author.IsBot && message.IsPinned;
		}

		private static async Task ReactToEmote(SocketReaction reaction, IUserMessage message, IEmote emote, string messagePrefix)
		{
			if (reaction.Emote.Name == emote.Name)
			{
				string usernames = await GetUserNames(message, emote);
				await message!.ModifyAsync(prop => prop.EditContent(messagePrefix, message.Content, usernames));
			}
		}

		private static async Task<string> GetUserNames(IUserMessage message, IEmote emote)
		{
			var users = await message.GetReactionUsersAsync(emote, 20).FlattenAsync();
			var userNames = users.Where(u => !u.IsBot).Select(u => u is IGuildUser user ? user.Nickname : u.Username);
			return userNames.Any() ? string.Join(", ", userNames) : "TBD";
		}
	}
}
