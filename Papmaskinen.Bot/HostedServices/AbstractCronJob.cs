using Cronos;
using Microsoft.Extensions.Logging;

namespace Papmaskinen.Bot.HostedServices;

public abstract class AbstractCronJob(ILogger<AbstractCronJob> logger, string expression) : AbstractHostedService(logger)
{
	private readonly CronExpression expression = CronExpression.Parse(expression);

	protected TimeZoneInfo TimeZoneInfo { get; } = TimeZoneInfo.Local;

	protected override async Task ScheduleJob(CancellationToken cancellationToken)
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
		}
	}
}