using Discord;
using Discord.WebSocket;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot
{
	public class Function
	{
		private const string ScheduledMessage = @"
- Next scheduled meet up: {0}
- Place: TBD 
- Time: 17:00
- Game Master: TBD
- Primary game: TBD
- Game suggestions: reply to this message with wishes/suggestions

Attending:
TBD
Hopefully:
TBD

You can use the following reactions to interact with this message.
:thumbsup: \: Attending
:fingers_crossed: \: Hopefully attending
:thumbsdown: \: Not attending
:house: \: I can host
:game_die: \: I can be Game Master
@everyone";

		private readonly DiscordSettings settings;
		private readonly DiscordSocketClient client;

		public Function(IOptionsMonitor<DiscordSettings> settings, DiscordSocketClient client)
		{
			this.settings = settings.CurrentValue;
			this.client = client;
		}

		public async Task CronFunction([TimerTrigger("%Discord:NextEvent:Schedule%", RunOnStartup = false)] TimerInfo timer)
		{
			var channel = await this.client.GetChannelAsync(this.settings.NextEvent.ChannelId);
			if (channel is ITextChannel ch)
			{
				var pinnedMessages = await ch.GetPinnedMessagesAsync();
				foreach (var pinnedMessage in pinnedMessages.Where(m => m.Author.IsBot))
				{
					if (pinnedMessage is IUserMessage userMessage)
					{
						await userMessage.UnpinAsync();
					}
				}

				DateTime now = DateTime.Now;
				DateTime lastDayOfMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);
				string date = this.FindFriday(lastDayOfMonth).ToString("D");
				var message = await ch.SendMessageAsync(string.Format(ScheduledMessage, date), allowedMentions: new AllowedMentions(AllowedMentionTypes.Everyone));
				var reactions = new List<IEmote>
				{
					Emotes.ThumbsUp,
					Emotes.FingersCrossed,
					Emotes.ThumbsDown,
					Emotes.House,
					Emotes.GameDie,
				};
				await message.AddReactionsAsync(reactions);
				await message.PinAsync();
			}
		}

		private DateTime FindFriday(DateTime date)
		{
			return date.DayOfWeek == DayOfWeek.Friday ? date : this.FindFriday(date.AddDays(-1));
		}
	}
}
