using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Papmaskinen.Bot.HostedServices;

public abstract class AbstractCronJob : IHostedService, IDisposable
{
	private readonly ILogger<AbstractCronJob> logger;
	private readonly CronExpression expression;
	private Timer? timer = null;

	public AbstractCronJob(ILogger<AbstractCronJob> logger, string expression)
	{
		this.logger = logger;
		this.expression = CronExpression.Parse(expression);
		this.TimeZoneInfo = TimeZoneInfo.Local;
	}

	protected TimeZoneInfo TimeZoneInfo { get; }

	public Task StartAsync(CancellationToken cancellationToken)
	{
		return this.ScheduleJob(cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		this.logger.LogInformation("Timed Hosted Service is stopping.");

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
		this.logger.LogInformation("No job has been initiated");
		return Task.CompletedTask;
	}

	private async Task ScheduleJob(CancellationToken cancellationToken)
	{
		var next = this.expression.GetNextOccurrence(DateTimeOffset.Now, this.TimeZoneInfo);
		if (next.HasValue)
		{
			string date = next.Value.ToLocalTime().ToString("f");
			this.logger.LogInformation("{Name}'s next job is scheduled for {date}", this.GetType().Name, date);
			var delay = next.Value - DateTimeOffset.Now;
			if (delay.TotalMilliseconds <= 0)
			{
				await this.ScheduleJob(cancellationToken);
			}

			this.timer = new Timer(
				async (state) =>
				{
					this.timer?.Dispose();
					this.timer = null;

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
