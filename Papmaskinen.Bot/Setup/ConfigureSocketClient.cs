using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Events;

namespace Papmaskinen.Bot.Setup;

public class ConfigureSocketClient(
	ILogger<ConfigureSocketClient> logger,
	DiscordSocketClient client,
	IOptionsMonitor<DiscordSettings> settings,
	Reactions reactions,
	Ready ready,
	SubmittedModals submittedModals,
	Messages messages)
{
	private readonly DiscordSettings settings = settings.CurrentValue;

	internal async Task Setup()
	{
		client.Log += msg =>
		{
			logger.LogInformation("{Message}", msg);
			return Task.CompletedTask;
		};

		client.Ready += ready.InstallCommands;
		client.SlashCommandExecuted += SlashCommands.SlashCommandReceived;
		client.MessageReceived += messages.MessageReceived;
		client.ReactionAdded += reactions.RemoveBotPostReaction;
		client.ReactionAdded += reactions.NextEventReactions;
		client.ReactionAdded += reactions.NominationReactions;
		client.ReactionRemoved += reactions.NextEventReactions;
		client.ReactionRemoved += reactions.NominationReactions;
		client.ModalSubmitted += submittedModals.ModalSubmitted;

		await client.LoginAsync(TokenType.Bot, this.settings.BotToken);
		await client.StartAsync();
	}

	internal async Task Disconnect()
	{
		await client.StopAsync();
	}
}
