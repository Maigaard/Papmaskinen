namespace Papmaskinen.Bot.Setup
{
	public class DiscordSettings
	{
		public string? BotToken { get; set; }

		public Schedule NextEvent { get; set; } = new();

		public class Schedule
		{
			public ulong ChannelId { get; set; } = 0;
		}
	}
}
