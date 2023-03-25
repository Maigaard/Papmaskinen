using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Papmaskinen.Integrations.BoardGameGeek.Services;
using Papmaskinen.Integrations.Http.Services;
using Papmaskinen.Integrations.Http.Services.Implementation;
using Papmaskinen.Integrations.Http.Services.SerializerSettings;

namespace Papmaskinen.Integrations.BoardGameGeek.Configuration;

public static class BoardGameGeekConfiguration
{
	public static void AddBoardGameGeek(this IServiceCollection services, Action<BoardGameGeekOptions> configure)
	{
		services.Configure(configure);
		services.AddSingleton<ISerializer<XmlSettings>, XmlSerializer>();
		services.AddHttpClient<BoardGameGeekHttpClient>((provider, client) =>
		{
			var options = provider.GetRequiredService<IOptionsMonitor<BoardGameGeekOptions>>().CurrentValue;
			client.BaseAddress = new Uri(options.Url);
		});
		services.AddSingleton<BoardGameGeekService>();
	}
}