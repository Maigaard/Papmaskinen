using System.Net;

namespace Papmaskinen.Integrations.Http.Services.Implementation;

public class SimpleHttpClient<TSettings>(HttpClient httpClient, ISerializer<TSettings> serializer) : ISimpleHttpClient
	where TSettings : class
{
	public virtual async Task<bool> DeleteAsync(string url)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			throw new ArgumentException("message", nameof(url));
		}

		using var response = await this.MakeRequestAsync(url, HttpMethod.Delete);
		try
		{
			await this.ValidateResponseAsync(response, defaultIfNotFound: false);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public virtual async Task<TResult?> GetAsync<TResult>(string url, bool defaultIfNotFound = false)
	{
		using var response = await this.GetRequestAsync(url);
		await this.ValidateResponseAsync(response, defaultIfNotFound);

		return await serializer.DeserializeAsync<TResult>(response, defaultIfNotFound);
	}

	public virtual async Task<TResult?> PostAsync<TResult>(string url, object? data)
	{
		HttpContent? content = data == null ? null : serializer.Serialize(data);

		using var response = await this.MakeRequestAsync(url, HttpMethod.Post, content);
		await this.ValidateResponseAsync(response, defaultIfNotFound: false);
		TResult? result = await serializer.DeserializeAsync<TResult>(response);

		return result;
	}

	public virtual async Task<TResult?> PutAsync<TResult>(string url, object data)
	{
		ArgumentNullException.ThrowIfNull(data);

		var content = serializer.Serialize(data);
		using var response = await this.MakeRequestAsync(url, HttpMethod.Put, content);
		await this.ValidateResponseAsync(response, defaultIfNotFound: true);
		TResult? result = await serializer.DeserializeAsync<TResult>(response);

		return result;
	}

	public virtual async Task<HttpResponseMessage> MakeRequestAsync(string url, HttpMethod method, HttpContent? content = null)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			throw new ArgumentException("Parameter cannot be null or white space", nameof(url));
		}

		var message = new HttpRequestMessage()
		{
			RequestUri = new Uri(url, UriKind.Relative),
			Method = method,
			Content = content,
		};

		var response = await httpClient.SendAsync(message);

		return response;
	}

	public virtual Task ValidateResponseAsync(HttpResponseMessage response, bool defaultIfNotFound)
	{
		if (!defaultIfNotFound || response.StatusCode != HttpStatusCode.NotFound)
		{
			response.EnsureSuccessStatusCode();
		}

		return Task.CompletedTask;
	}

	protected virtual Task<HttpResponseMessage> GetRequestAsync(string url)
	{
		return this.MakeRequestAsync(url, HttpMethod.Get);
	}
}