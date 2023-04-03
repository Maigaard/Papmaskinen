using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.HostedServices;

public class NextEvent : AbstractCronJob
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

	private readonly ILogger<NextEvent> logger;
	private readonly DiscordSettings settings;
	private readonly DiscordSocketClient client;

	public NextEvent(ILogger<NextEvent> logger, IOptionsMonitor<DiscordSettings> settings, DiscordSocketClient client)
		: base(logger, settings.CurrentValue.NextEvent.Schedule)
	{
		this.logger = logger;
		this.settings = settings.CurrentValue;
		this.client = client;
	}

	protected override async Task DoWork(object? state)
	{
		await this.UpdateUserNicknames();
		var channel = await this.client.GetChannelAsync(this.settings.NextEvent.ChannelId);
		if (channel is ITextChannel ch)
		{
			await UnpinMessages(ch);

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

	private static async Task UnpinMessages(ITextChannel ch)
	{
		var pinnedMessages = await ch.GetPinnedMessagesAsync();
		foreach (var pinnedMessage in pinnedMessages.Where(m => m.Author.IsBot))
		{
			if (pinnedMessage is IUserMessage userMessage)
			{
				await userMessage.UnpinAsync();
			}
		}
	}

	private async Task UpdateUserNicknames()
	{
		int currentMonth = DateTime.Now.Month;
		await this.client.Guilds.First().DownloadUsersAsync();
		var guild = this.client.Guilds.First();
		foreach (var user in guild.Users)
		{
			this.logger.LogInformation("Attempting to nickname {DisplayName}", user.DisplayName);
			await user.UpdateNickName(suffix: Emotes.Clocks[currentMonth].ToString());
		}
	}

	private DateTime FindFriday(DateTime date)
	{
		return date.DayOfWeek == DayOfWeek.Friday ? date : this.FindFriday(date.AddDays(-1));
	}
}
