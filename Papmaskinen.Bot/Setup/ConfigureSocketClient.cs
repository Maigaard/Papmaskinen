using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Events;

namespace Papmaskinen.Bot.Setup;

public class ConfigureSocketClient
{
	private readonly ILogger<ConfigureSocketClient> logger;
	private readonly DiscordSocketClient client;
	private readonly Reactions reactions;
	private readonly Ready ready;
	private readonly SubmittedModals submittedModals;
	private readonly Messages messages;
	private readonly DiscordSettings settings;

	public ConfigureSocketClient(
		ILogger<ConfigureSocketClient> logger,
		DiscordSocketClient client,
		IOptionsMonitor<DiscordSettings> settings,
		Reactions reactions,
		Ready ready,
		SubmittedModals submittedModals,
		Messages messages)
	{
		this.logger = logger;
		this.client = client;
		this.reactions = reactions;
		this.ready = ready;
		this.submittedModals = submittedModals;
		this.messages = messages;
		this.settings = settings.CurrentValue;
	}

	internal async Task Setup()
	{
		this.client.Log += (LogMessage msg) =>
		{
			this.logger.LogInformation("{message}", msg);
			return Task.CompletedTask;
		};

		this.client.Ready += this.ready.InstallCommands;
		this.client.SlashCommandExecuted += SlashCommands.SlashCommandReceived;
		this.client.MessageReceived += this.messages.MessageReceived;
		this.client.ReactionAdded += this.reactions.RemoveBotPostReaction;
		this.client.ReactionAdded += this.reactions.NextEventReactions;
		this.client.ReactionAdded += this.reactions.NominationReactions;
		this.client.ReactionRemoved += this.reactions.NextEventReactions;
		this.client.ReactionRemoved += this.reactions.NominationReactions;
		this.client.ModalSubmitted += this.submittedModals.ModalSubmitted;

		await this.client.LoginAsync(TokenType.Bot, this.settings.BotToken);
		await this.client.StartAsync();
	}

	internal async Task Disconnect()
	{
		await this.client.StopAsync();
	}
}
