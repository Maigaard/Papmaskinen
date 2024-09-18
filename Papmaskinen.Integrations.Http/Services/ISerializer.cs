namespace Papmaskinen.Integrations.Http.Services;

public interface ISerializer<in TSettings>
	where TSettings : class
{
	HttpContent Serialize<TContent>(TContent data, TSettings? settings = null)
		where TContent : class;

	Task<TResult?> DeserializeAsync<TResult>(HttpResponseMessage response, bool defaultIfNotFound = false);
}
