using Papmaskinen.Integrations.Http.Services;
using Papmaskinen.Integrations.Http.Services.Implementation;
using Papmaskinen.Integrations.Http.Services.SerializerSettings;

namespace Papmaskinen.Integrations.BoardGameGeek.Services;

public class BoardGameGeekHttpClient : SimpleHttpClient<XmlSettings>
{
	public BoardGameGeekHttpClient(HttpClient httpClient, ISerializer<XmlSettings> serializer)
		: base(httpClient, serializer)
	{
	}
}