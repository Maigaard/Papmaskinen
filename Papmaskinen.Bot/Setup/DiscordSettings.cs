namespace Papmaskinen.Bot.Setup;

public class DiscordSettings
{
	public string? BotToken { get; set; }

	public ulong BotId { get; set; } = 0;

	public ulong GuildId { get; set; } = 0;

	public CronSchedule NextEvent { get; set; } = new();

	public CronSchedule Nominations { get; set; } = new();

	public class CronSchedule
	{
		public ulong ChannelId { get; set; } = 0;

		public string Schedule { get; set; } = null!;
	}
}
