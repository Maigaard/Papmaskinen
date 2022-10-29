﻿namespace Papmaskinen.Bot.Setup
{
	public class DiscordSettings
	{
		public string? BotToken { get; set; }

		public ulong BotId { get; set; } = 0;

		public Schedule NextEvent { get; set; } = new();

		public class Schedule
		{
			public ulong ChannelId { get; set; } = 0;
		}
	}
}