using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Papmaskinen.Bot.Events;

namespace Papmaskinen.Bot.Setup;

public class ConfigureSocketClient
{
	private readonly DiscordSocketClient client;
	private readonly Reactions reactions;
	private readonly SlashCommands slashCommands;
	private readonly Ready ready;
	private readonly SubmittedModals submittedModals;
	private readonly Messages messages;
	private readonly DiscordSettings settings;

	public ConfigureSocketClient(
		DiscordSocketClient client,
		IOptionsMonitor<DiscordSettings> settings,
		Reactions reactions,
		SlashCommands slashCommands,
		Ready ready,
		SubmittedModals submittedModals,
		Messages messages)
	{
		this.client = client;
		this.reactions = reactions;
		this.slashCommands = slashCommands;
		this.ready = ready;
		this.submittedModals = submittedModals;
		this.messages = messages;
		this.settings = settings.CurrentValue;
	}

	internal async Task Setup()
	{
		this.client.Log += (LogMessage msg) =>
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		};

		this.client.Ready += this.ready.InstallCommands;
		this.client.SlashCommandExecuted += this.slashCommands.SlashCommandReceived;
		this.client.MessageReceived += this.messages.MessageReceived;
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
