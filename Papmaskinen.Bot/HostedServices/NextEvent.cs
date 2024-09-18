using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Bot.Models.Constants;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.HostedServices;

public class NextEvent(
	ILogger<NextEvent> logger,
	IOptionsMonitor<DiscordSettings> settings,
	DiscordSocketClient client)
	: AbstractCronJob(logger, settings.CurrentValue.NextEvent.Schedule)
{
	private readonly DiscordSettings settings = settings.CurrentValue;

	protected override async Task DoWork(CancellationToken cancellationToken)
	{
		await this.UpdateUserNicknames();
		var channel = await client.GetChannelAsync(this.settings.NextEvent.ChannelId);
		if (channel is ITextChannel ch)
		{
			await UnpinMessages(ch);

			DateTimeOffset now = DateTimeOffset.Now;
			DateTimeOffset lastDayOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, this.TimeZoneInfo.BaseUtcOffset)
				.AddMonths(1)
				.AddDays(-1);
			DateTimeOffset date = FindFriday(lastDayOfMonth);
			var message = await ch.SendMessageAsync(
				string.Format(MessageTemplates.NextEventMessage, date.ToString("D")),
				allowedMentions: new AllowedMentions(AllowedMentionTypes.Everyone));

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
			await this.InitGameUpdate(date, message.Id, cancellationToken);
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

	private static DateTimeOffset FindFriday(DateTimeOffset date)
	{
		while (true)
		{
			if (date.DayOfWeek == DayOfWeek.Friday)
			{
				return date;
			}

			date = date.AddDays(-1);
		}
	}

	private async Task UpdateUserNicknames()
	{
		int currentMonth = DateTime.Now.Month;
		await client.Guilds.First().DownloadUsersAsync();
		var guild = client.Guilds.First();
		foreach (var user in guild.Users)
		{
			logger.LogInformation("Attempting to nickname {DisplayName}", user.DisplayName);
			await user.UpdateNickName(suffix: Emotes.Clocks[currentMonth - 1].ToString());
		}
	}

	private async Task InitGameUpdate(DateTimeOffset date, ulong messageId, CancellationToken cancellationToken)
	{
		DateTimeOffset weekFromDate = date.AddDays(-7);
		UpdateNextEventGame service = new(logger, weekFromDate, messageId, this.settings, client);
		await service.StartAsync(cancellationToken);
	}
}
