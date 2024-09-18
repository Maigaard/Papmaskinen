using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Papmaskinen.Bot.HostedServices;

public abstract class AbstractCronJob(ILogger<AbstractCronJob> logger, string expression) : IHostedService, IDisposable
{
	private readonly CronExpression expression = CronExpression.Parse(expression);
	private Timer? timer;

	protected TimeZoneInfo TimeZoneInfo { get; } = TimeZoneInfo.Local;

	public Task StartAsync(CancellationToken cancellationToken)
	{
		return this.ScheduleJob(cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("Timed Hosted Service is stopping.");

		this.timer?.Change(Timeout.Infinite, 0);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		this.Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		this.timer?.Dispose();
	}

	protected virtual Task DoWork(CancellationToken cancellationToken)
	{
		logger.LogInformation("No job has been initiated");
		return Task.CompletedTask;
	}

	private async Task ScheduleJob(CancellationToken cancellationToken)
	{
		var next = this.expression.GetNextOccurrence(DateTimeOffset.Now, this.TimeZoneInfo);
		if (next.HasValue)
		{
			string date = next.Value.ToLocalTime().ToString("f");
			logger.LogInformation("{Name}'s next job is scheduled for {Date}", this.GetType().Name, date);
			var delay = next.Value - DateTimeOffset.Now;
			if (delay.TotalMilliseconds <= 0)
			{
				await this.ScheduleJob(cancellationToken);
			}

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
						await this.ScheduleJob(cancellationToken);
					}
				},
				null,
				(uint)delay.TotalMilliseconds,
				Timeout.Infinite);
		}
	}
}