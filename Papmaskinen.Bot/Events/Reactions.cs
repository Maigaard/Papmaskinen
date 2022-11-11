using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.Events
{
	public class Reactions
	{
		private const string NominationPinnedMessage = @"
This place is for nominating and voting on games for future PapClub events.

Procedure: To nominate a game use command 'nominate', and add a boardgamegeek link to your game. 

Every month each member has one vote, to be used on any of the nominations.
A week before an event the highest voted nomination is chosen as the primary game.

Voting Emoji: (\:clockX\: where X is the current month)
January: 🕐
February: 🕑
March: 🕒
April: 🕓
May: 🕔 
June: 🕕 
July: 🕖 
August: 🕗
September: 🕘
October: 🕙
November: 🕚
December: 🕛

Have fun! 😁 🎫

Votes:
";

		private readonly DiscordSettings settings;

		public Reactions(IOptionsMonitor<DiscordSettings> options)
		{
			this.settings = options.CurrentValue;
		}

		internal async Task NextEventReactions(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
		{
			IUserMessage message = await messageCache.GetOrDownloadAsync();
			if (channelCache.Id == this.settings.Nominations.ChannelId &&
				reaction.UserId != this.settings.BotId &&
				message.Author.IsBot)
			{
				await ReactToEmote(reaction, message!, Emotes.ThumbsUp, "Attending");
				await ReactToEmote(reaction, message!, Emotes.FingersCrossed, "Hopefully");
				await ReactToEmote(reaction, message!, Emotes.House, "- Place");
				await ReactToEmote(reaction, message!, Emotes.GameDie, "- Game Master");
			}
		}

		internal async Task NominationReactions(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
		{
			if (channelCache.Id == this.settings.Nominations.ChannelId &&
				reaction.UserId != this.settings.BotId &&
				Emotes.Clocks.Any(c => c.Name == reaction.Emote.Name))
			{
				IUserMessage message = await messageCache.GetOrDownloadAsync();
				IMessageChannel channel = await channelCache.GetOrDownloadAsync();
				var pinnedMessages = await channel.GetPinnedMessagesAsync();

				if (pinnedMessages.FirstOrDefault() is not IUserMessage pinnedMessage)
				{
					pinnedMessage = await channel.SendMessageAsync(NominationPinnedMessage);
					await pinnedMessage.PinAsync();
				}

				string nominationTitle = Regex.Match(message.Content, @"^([\w:\- ]+)[\r\n]*").Value;
				int nominationVotes = message.Reactions.Where(r => Emotes.Clocks.Any(c => c.Name == r.Key.Name)).Sum(r => r.Value.ReactionCount);

				var titleRegex = new Regex($@"^({nominationTitle} : )([0-9]{{1,3}})", RegexOptions.Multiline);
				string newContent = string.Empty;
				if (titleRegex.IsMatch(pinnedMessage.Content))
				{
					newContent = titleRegex.Replace(pinnedMessage.Content, $"{nominationTitle} : {nominationVotes}");
				}
				else
				{
					newContent = $"{pinnedMessage.Content}\r\n{nominationTitle} : {nominationVotes}";
				}

				await pinnedMessage.ModifyAsync(prop => prop.Content = newContent);
			}
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
