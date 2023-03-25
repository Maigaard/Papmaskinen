using System.Text;
using Newtonsoft.Json;
using Papmaskinen.Integrations.Http.Services.SerializerSettings;

namespace Papmaskinen.Integrations.Http.Services.Implementation;

public class JsonSerializer : AbstractSerializer<JsonSettings>, ISerializer<JsonSettings>
{
	public override HttpContent Serialize<TContent>(TContent data, JsonSettings? settings = null)
	{
		string serializedData = string.Empty;
		if (data != null)
		{
			serializedData = JsonConvert.SerializeObject(data);
		}

		return new StringContent(serializedData, Encoding.UTF8, "application/json");
	}

	protected override TResult? Deserialize<TResult>(string responseText)
		where TResult : default
	{
		return JsonConvert.DeserializeObject<TResult?>(responseText) ?? default;
	}
}
