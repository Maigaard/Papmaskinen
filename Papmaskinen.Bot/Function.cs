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
\|\|
Hopefully:
\|\|
If not meantioned but you'd still like to attend please confirm to the role call below.
@everyone";

		private readonly DiscordSettings settings;
		private readonly DiscordSocketClient client;

		public Function(IOptionsMonitor<DiscordSettings> settings, DiscordSocketClient client)
		{
			this.settings = settings.CurrentValue;
			this.client = client;
		}

		public async Task CronFunction([TimerTrigger("%Discord:NextEvent:Schedule%", RunOnStartup = true)] TimerInfo timer)
		{
			var channel = await this.client.GetChannelAsync(this.settings.NextEvent.ChannelId);
			if (channel is ITextChannel ch)
			{
				DateTime now = DateTime.Now;
				DateTime lastDayOfMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);
				string date = this.FindFriday(lastDayOfMonth).ToString("D");
				await ch.SendMessageAsync(string.Format(ScheduledMessage, date), allowedMentions: new AllowedMentions(AllowedMentionTypes.Everyone));
			}
		}

		private DateTime FindFriday(DateTime date)
		{
			return date.DayOfWeek == DayOfWeek.Friday ? date : this.FindFriday(date.AddDays(-1));
		}
	}
}
