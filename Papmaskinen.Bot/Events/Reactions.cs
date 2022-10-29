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

		internal async Task Added(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
		{
			IMessage message = messageCache.Value;
			if (channel.Value is ITextChannel ch && channel.Id == this.settings.NextEvent.ChannelId)
			{
				if (messageCache.HasValue == false)
				{
					message = await channel.Value.GetMessageAsync(messageCache.Id);
				}

				if (message is IUserMessage userMessage && (reaction.Emote.Name == "👍" || reaction.Emote.Name == "👎"))
				{
					string userName = reaction.User.Value is IGuildUser guildUser ? guildUser.Nickname : reaction.User.Value.Username;
					await userMessage.ModifyAsync((prop) => { prop.Content = $"{prop.Content} Attending: ${userName}"; });
				}
			}
		}

		internal Task Removed(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
		{
			return Task.CompletedTask;
		}
	}
}
