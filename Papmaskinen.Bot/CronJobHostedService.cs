using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Papmaskinen.Bot;

public abstract class CronJobHostedService : IHostedService, IDisposable
{
	private readonly TimeZoneInfo timeZoneInfo;
	private readonly ILogger<CronJobHostedService> logger;
	private readonly CronExpression expression;
	private Timer? timer = null;

	public CronJobHostedService(ILogger<CronJobHostedService> logger, string expression)
	{
		this.logger = logger;
		this.expression = CronExpression.Parse(expression);
		this.timeZoneInfo = TimeZoneInfo.Local;
	}

	public Task StartAsync(CancellationToken stoppingToken)
	{
		return this.ScheduleJob(stoppingToken);
	}

	public Task StopAsync(CancellationToken stoppingToken)
	{
		this.logger.LogInformation("Timed Hosted Service is stopping.");

		this.timer?.Change(Timeout.Infinite, 0);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		this.timer?.Dispose();
	}

	protected virtual Task DoWork(object? state)
	{
		this.logger.LogInformation("No job has been initiated");
		return Task.CompletedTask;
	}

	private async Task ScheduleJob(CancellationToken stoppingToken)
	{
		var next = this.expression.GetNextOccurrence(DateTimeOffset.Now, this.timeZoneInfo);
		if (next.HasValue)
		{
			string date = next.Value.ToLocalTime().ToString("f");
			this.logger.LogInformation("{Name}'s next job is scheduled for {date}", this.GetType().Name, date);
			var delay = next.Value - DateTimeOffset.Now;
			if (delay.TotalMilliseconds <= 0)
			{
				await this.ScheduleJob(stoppingToken);
			}

			this.timer = new Timer(
				async (state) =>
				{
					this.timer?.Dispose();
					this.timer = null;

					if (!stoppingToken.IsCancellationRequested)
					{
						await this.DoWork(stoppingToken);
						await this.ScheduleJob(stoppingToken);
					}
				},
				null,
				(uint)delay.TotalMilliseconds,
				Timeout.Infinite);
		}

		await Task.CompletedTask;
	}
}
