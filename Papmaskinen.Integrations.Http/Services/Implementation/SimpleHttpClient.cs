using System.Net;

namespace Papmaskinen.Integrations.Http.Services.Implementation
{
	public class SimpleHttpClient<TSettings> : ISimpleHttpClient
		where TSettings : class
	{
		private readonly HttpClient httpClient;
		private readonly ISerializer<TSettings> serializer;

		public SimpleHttpClient(HttpClient httpClient, ISerializer<TSettings> serializer)
		{
			this.httpClient = httpClient;
			this.serializer = serializer;
		}

		public virtual async Task<bool> DeleteAsync(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				throw new ArgumentException("message", nameof(url));
			}

			using (var response = await this.MakeRequestAsync(url, HttpMethod.Delete))
			{
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
		}

		public virtual async Task<TResult> GetAsync<TResult>(string url, bool defaultIfNotFound = false)
		{
			using (var response = await this.GetRequestAsync(url))
			{
				await this.ValidateResponseAsync(response, defaultIfNotFound);

				return await this.serializer.DeserializeAsync<TResult>(response, defaultIfNotFound);
			}
		}

		public virtual async Task<TResult> PostAsync<TResult>(string url, object? data)
		{
			TResult? result = default;

			HttpContent content = data == null ? null : this.serializer.Serialize(data);

			using (var response = await this.MakeRequestAsync(url, HttpMethod.Post, content))
			{
				await this.ValidateResponseAsync(response, defaultIfNotFound: false);
				result = await this.serializer.DeserializeAsync<TResult>(response);
			}

			return result;
		}

		public virtual async Task PutAsync(string url, object data)
		{
			HttpContent content = data == null ? null : this.serializer.Serialize(data);

			using (var response = await this.MakeRequestAsync(url, HttpMethod.Put, content))
			{
				await this.ValidateResponseAsync(response, defaultIfNotFound: true);
			}
		}

		public virtual async Task<TResult> PutAsync<TResult>(string url, object data)
		{
			TResult result = default;

			if (data is null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var content = this.serializer.Serialize(data);
			using (var response = await this.MakeRequestAsync(url, HttpMethod.Put, content))
			{
				await this.ValidateResponseAsync(response, defaultIfNotFound: true);
				result = await this.serializer.DeserializeAsync<TResult>(response);
			}

			return result;
		}

		public virtual async Task<HttpResponseMessage> MakeRequestAsync(string url, HttpMethod method, HttpContent content = null)
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

			var response = await this.httpClient.SendAsync(message);

			return response;
		}

		public virtual async Task ValidateResponseAsync(HttpResponseMessage response, bool defaultIfNotFound)
		{
			try
			{
				if (!defaultIfNotFound || response.StatusCode != HttpStatusCode.NotFound)
				{
					response.EnsureSuccessStatusCode();
				}
			}
			catch (Exception)
			{
				// var body = await response.Content.ReadAsStringAsync();
				// this.logger.LogError(ex, $"The API returned the following message: ({(int)response.StatusCode}){response.ReasonPhrase}: {body}");
				throw;
			}
		}

		protected virtual Task<HttpResponseMessage> GetRequestAsync(string url)
		{
			return this.MakeRequestAsync(url, HttpMethod.Get);
		}
	}
}
