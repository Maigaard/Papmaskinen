using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Papmaskinen.Bot.HostedServices;

public abstract class AbstractHostedService(ILogger logger) : IHostedService, IDisposable
{
	protected virtual Timer? Timer { get; set; }

	public void SetTimer(TimeSpan delay, CancellationToken cancellationToken)
	{
		this.Timer = new Timer(
			async _ =>
			{
				if (this.Timer != null)
				{
					await this.Timer.DisposeAsync();
					this.Timer = null;
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

	public virtual Task StartAsync(CancellationToken cancellationToken)
	{
		return this.ScheduleJob(cancellationToken);
	}

	public virtual Task StopAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("Timed Hosted Service is stopping.");

		this.Timer?.Change(Timeout.Infinite, 0);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		this.Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected abstract Task ScheduleJob(CancellationToken cancellationToken);

	protected virtual Task DoWork(CancellationToken cancellationToken)
	{
		logger.LogInformation("No job has been initiated");
		return Task.CompletedTask;
	}

	protected virtual void Dispose(bool disposing)
	{
		this.Timer?.Dispose();
	}
}