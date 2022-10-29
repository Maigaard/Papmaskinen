namespace Papmaskinen.Bot.Services
{
	public class DiscordSettings
	{
		public string? BotToken { get; set; }

		public Schedule NextEvent { get; set; }

		public class Schedule
		{
			public ulong ChannelId { get; set; } = 0;
		}
	}
}
