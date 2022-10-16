using System.Net;
using Discord.Rest;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Papmaskinen.App
{
	public class Interactions
	{
		private readonly ILogger logger;
		private readonly DiscordRestClient client;
		private readonly IConfiguration configuration;

		public Interactions(ILoggerFactory loggerFactory, DiscordRestClient client, IConfiguration configuration)
		{
			this.logger = loggerFactory.CreateLogger<Interactions>();
			this.client = client;
			this.configuration = configuration;
		}

		[Function("Interactions")]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = nameof(Interactions))] HttpRequestData req)
		{
			req.Headers.TryGetValues("X-Signature-Ed25519", out IEnumerable<string>? signatures);
			req.Headers.TryGetValues("X-Signature-Timestamp", out IEnumerable<string>? timestamps);
			using StreamReader reader = new(req.Body);
			string publicKey = this.configuration.GetValue<string>("Discord.App.PublicKey");
			try
			{
				RestInteraction interaction = await this.client.ParseHttpInteractionAsync(publicKey, signatures?.SingleOrDefault(), timestamps?.SingleOrDefault(), reader.ReadToEnd());
				HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

				if (interaction != null)
				{
					switch (interaction.Type)
					{
						case Discord.InteractionType.Ping:
							await response.WriteAsJsonAsync(new { type = Discord.InteractionResponseType.Pong });
							break;
						case Discord.InteractionType.ApplicationCommand:

							break;
						case Discord.InteractionType.MessageComponent:
							break;
						case Discord.InteractionType.ApplicationCommandAutocomplete:
							break;
						case Discord.InteractionType.ModalSubmit:
							break;
						default:
							break;
					}
				}

				return response;
			}
			catch (Exception)
			{
				HttpResponseData response = req.CreateResponse(HttpStatusCode.Unauthorized);
				response.WriteString("Failed to verify request");
				return response;
			}
		}
	}
}
