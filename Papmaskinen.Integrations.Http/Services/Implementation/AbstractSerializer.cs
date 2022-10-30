using System.Net;
using System.Runtime.InteropServices;

namespace Papmaskinen.Integrations.Http.Services.Implementation
{
	public abstract class AbstractSerializer<TSettings> : ISerializer<TSettings>
		where TSettings : class
	{
		public virtual async Task<TResult?> DeserializeAsync<TResult>(HttpResponseMessage response, bool defaultIfNotFound = false)
		{
			if (response.StatusCode == HttpStatusCode.NotFound && defaultIfNotFound)
			{
				return default;
			}
			else
			{
				var responseText = await response.Content.ReadAsStringAsync();

				return this.Deserialize<TResult>(responseText);
			}
		}

		public abstract HttpContent Serialize<TContent>(TContent data, TSettings? settings = null)
			where TContent : class;

		protected abstract TResult? Deserialize<TResult>(string responseText);
	}
}
