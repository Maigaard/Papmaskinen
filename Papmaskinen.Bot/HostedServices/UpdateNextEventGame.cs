using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.HostedServices;

internal partial class UpdateNextEventGame : IHostedService, IDisposable
{
	private readonly ILogger<NextEvent> logger;
	private readonly DateTimeOffset date;
	private readonly ulong messageId;
	private readonly DiscordSettings settings;
	private readonly DiscordSocketClient client;
	private Timer? timer = null;

	public UpdateNextEventGame(
		ILogger<NextEvent> logger,
		DateTimeOffset date,
		ulong messageId,
		DiscordSettings settings,
		DiscordSocketClient client)
	{
		this.logger = logger;
		this.date = date;
		this.messageId = messageId;
		this.settings = settings;
		this.client = client;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		TimeSpan delay = this.date - DateTimeOffset.Now;
		this.logger.LogInformation("Starting event update in: {delay} ms", delay.TotalMilliseconds);

		this.timer = new Timer(
			async (state) =>
			{
				this.timer?.Dispose();
				this.timer = null;

				if (!cancellationToken.IsCancellationRequested)
				{
					await this.DoWork(cancellationToken);
				}
			},
			null,
			(uint)delay.TotalMilliseconds,
			Timeout.Infinite);

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		this.logger.LogInformation("Timed Hosted Service is stopping.");

		this.timer?.Change(Timeout.Infinite, 0);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		this.timer?.Dispose();
		GC.SuppressFinalize(this);
	}

	[GeneratedRegex("^([\\w: -]+) : ([0-9]{1,3})", RegexOptions.Multiline)]
	private static partial Regex GetNominationVotesRegex();

	private async Task DoWork(CancellationToken cancellationToken)
	{
		if (await this.client.GetChannelAsync(this.settings.NextEvent.ChannelId) is ITextChannel nextEventChannel
			&& await this.client.GetChannelAsync(this.settings.Nominations.ChannelId) is ITextChannel nominationChannel
			&& (await nominationChannel.GetPinnedMessagesAsync()).FirstOrDefault() is IUserMessage nominationVoteMessage)
		{
			var gameVotes = GetNominationVotesRegex().Matches(nominationVoteMessage.Content)
				.Select(m => new { Name = m.Groups[1].Value, Votes = int.Parse(m.Groups[2].Value) });
			IMessage message = await nextEventChannel.GetMessageAsync(this.messageId);
			if (message is IUserMessage nextEventMessage && gameVotes.Any())
			{
				await nextEventMessage.ModifyAsync(prop =>
				prop.EditContent("- Primary game", nextEventMessage.Content, gameVotes.OrderByDescending(g => g.Votes).First().Name));
			}
		}

		this.logger.LogInformation("Starting event update");
		await this.StopAsync(cancellationToken);
	}
}