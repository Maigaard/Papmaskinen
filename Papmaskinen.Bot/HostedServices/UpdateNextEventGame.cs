using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Papmaskinen.Bot.Extensions;
using Papmaskinen.Bot.Setup;

namespace Papmaskinen.Bot.HostedServices;

internal sealed partial class UpdateNextEventGame(
	ILogger<NextEvent> logger,
	DateTimeOffset date,
	ulong messageId,
	DiscordSettings settings,
	DiscordSocketClient client)
	: IHostedService, IDisposable
{
	private Timer? timer = null;

	public Task StartAsync(CancellationToken cancellationToken)
	{
		TimeSpan delay = date - DateTimeOffset.Now;
		logger.LogInformation("Starting event update in: {Delay} ms", delay.TotalMilliseconds);

		this.timer = new Timer(
			async _ =>
			{
				if (this.timer != null)
				{
					await this.timer.DisposeAsync();
					this.timer = null;
				}

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
		logger.LogInformation("Timed Hosted Service is stopping.");

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
		if (await client.GetChannelAsync(settings.NextEvent.ChannelId) is ITextChannel nextEventChannel
			&& await client.GetChannelAsync(settings.Nominations.ChannelId) is ITextChannel nominationChannel
			&& (await nominationChannel.GetPinnedMessagesAsync()).FirstOrDefault() is IUserMessage nominationVoteMessage)
		{
			var gameVotes = GetNominationVotesRegex()
				.Matches(nominationVoteMessage.Content)
				.Select(m => new { Name = m.Groups[1].Value, Votes = int.Parse(m.Groups[2].Value) })
				.ToList();
			IMessage message = await nextEventChannel.GetMessageAsync(messageId);
			if (message is IUserMessage nextEventMessage && gameVotes.Any())
			{
				await nextEventMessage.ModifyAsync(
					prop =>
						prop.EditContent("- Primary game", nextEventMessage.Content, gameVotes.OrderByDescending(g => g.Votes).First().Name));
			}
		}

		logger.LogInformation("Starting event update");
		await this.StopAsync(cancellationToken);
	}
}