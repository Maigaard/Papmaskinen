namespace Papmaskinen.Integrations.Http.Services
{
	public interface ISimpleHttpClient
	{
		Task<TResult?> GetAsync<TResult>(string url, bool defaultIfNotFound = false);

		Task<TResult?> PostAsync<TResult>(string url, object data);

		Task<TResult?> PutAsync<TResult>(string url, object data);

		Task<bool> DeleteAsync(string url);

		Task<HttpResponseMessage> MakeRequestAsync(string url, HttpMethod method, HttpContent? content = null);

		Task ValidateResponseAsync(HttpResponseMessage response, bool defaultIfNotFound);
	}
}
