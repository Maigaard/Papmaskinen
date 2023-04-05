using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Bot.Models.Constants;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.Events;

public partial class Reactions
{
	private readonly DiscordSettings settings;

	public Reactions(IOptionsMonitor<DiscordSettings> options)
	{
		this.settings = options.CurrentValue;
	}

	internal async Task NextEventReactions(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
	{
		IUserMessage message = await messageCache.GetOrDownloadAsync();
		if (channelCache.Id == this.settings.NextEvent.ChannelId &&
			reaction.UserId != this.settings.BotId &&
			message?.Author?.IsBot == true)
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

			int nominationVotes = message.Reactions
				.Where(r => Emotes.Clocks.Any(c => c.Name == r.Key.Name))
				.Sum(r => r.Value.ReactionCount);
			await UpdateNominationVotes(message.Content, nominationVotes, channel);

			if (message.Author is SocketGuildUser user)
			{
				await user.UpdateNickName();
			}
		}
	}

	internal async Task RemoveBotPostReaction(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
	{
		if (channelCache.Id == this.settings.Nominations.ChannelId &&
			reaction.UserId != this.settings.BotId &&
			reaction.Emote.Name == Emotes.RedX.Name)
		{
			IUserMessage message = await messageCache.GetOrDownloadAsync();
			if (message.Interaction.User.Id == reaction.UserId)
			{
				IMessageChannel channel = await channelCache.GetOrDownloadAsync();
				await UpdateNominationVotes(message.Content, -1, channel);
				await message.DeleteAsync();
			}
		}
	}

	private static async Task UpdateNominationVotes(string nominationContent, int nominationVotes, IMessageChannel channel)
	{
		IUserMessage pinnedMessage = await GetPinnedMessage(channel);
		string nominationTitle = NominationTitle().Match(nominationContent).Value.TrimEnd();

		var titleRegex = new Regex($@"^({nominationTitle} : )([0-9]{{1,3}})", RegexOptions.Multiline);
		string nominationReplacement = nominationVotes >= 0
			? $"{nominationTitle} : {nominationVotes}"
			: string.Empty;

		if (titleRegex.IsMatch(pinnedMessage.Content))
		{
			await pinnedMessage.ModifyAsync(prop =>
				prop.Content = titleRegex.Replace(pinnedMessage.Content, nominationReplacement));
		}
		else if (!string.IsNullOrEmpty(nominationReplacement))
		{
			await pinnedMessage.ModifyAsync(prop =>
				prop.Content = $"{pinnedMessage.Content}\r\n{nominationReplacement}");
		}
	}

	private static async Task<IUserMessage> GetPinnedMessage(IMessageChannel channel)
	{
		var pinnedMessages = await channel.GetPinnedMessagesAsync();

		if (pinnedMessages.FirstOrDefault() is not IUserMessage pinnedMessage)
		{
			pinnedMessage = await channel.SendMessageAsync(MessageTemplates.NominationPinnedMessage);
			await pinnedMessage.PinAsync();
		}

		return pinnedMessage;
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
		var userNames = users.Where(u => !u.IsBot).Select(u => u is IGuildUser user ? user.DisplayName : u.Username);
		return userNames.Any() ? string.Join(", ", userNames) : "TBD";
	}

	[GeneratedRegex("^([\\w]+[\\w:\\- ]*)")]
	private static partial Regex NominationTitle();
}
