using System.Collections.Specialized;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Papmaskinen.Integrations.Http.Services;
using Papmaskinen.Integrations.Http.Services.Implementation;
using Papmaskinen.Integrations.Http.Services.SerializerSettings;
using Papmaskinen.Integrations.BoardGameGeek.Models;

namespace Papmaskinen.Integrations.BoardGameGeek
{
    public class BoardGameGeekHttpClient : SimpleHttpClient<XmlSettings>
    {
        public BoardGameGeekHttpClient(HttpClient httpClient, ISerializer<XmlSettings> serializer)
            : base(httpClient, serializer)
        {
        }
    }

    public class BoardGameGeekService
    {
        private readonly BoardGameGeekHttpClient client;
        public BoardGameGeekService(BoardGameGeekHttpClient client)
        {
            this.client = client;
        }

        public async Task<string> GetBoardGame(int boardGameId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("stats", "1");
            query.Add("id", boardGameId.ToString());

            var result = await this.client.GetAsync<ThingResultSet>($"thing&{query}");
            return result?.Items?.FirstOrDefault()?.Name?.FirstOrDefault(p => p.Type == "primary")?.Value ?? "Fandt ikke noget";
        }
    }

    public class BoardGameGeekOptions
    {
        public string Url { get; set; } = "https://boardgamegeek.com/xmlapi2";
    }

    public static class BoardGameGeekConfiguration
    {
        public static void AddBoardGameGeek(this IServiceCollection services, Action<BoardGameGeekOptions> configure)
        {
            services.Configure(configure);
            services.AddScoped<ISerializer<XmlSettings>, XmlSerializer>();
            services.AddHttpClient<BoardGameGeekHttpClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptionsMonitor<BoardGameGeekOptions>>().CurrentValue;
                client.BaseAddress = new Uri(options.Url);
            });
        }

    }
}