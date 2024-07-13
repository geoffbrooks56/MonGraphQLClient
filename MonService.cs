using System.Text.Json.Nodes;


namespace MonGraphQLClient;
internal static class MonService
{
	static string APIUrl { get; set; }
	static string APISecret { get; set; }

	static MonService()
	{
		APISecret = Environment.GetEnvironmentVariable("MondayAPIToken", EnvironmentVariableTarget.Process);
		APIUrl = Environment.GetEnvironmentVariable("MondayURL", EnvironmentVariableTarget.Process);
	}


	internal static async Task<JsonNode> GetResultAsync(Query query, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(nameof(query));

		string queryString = QueryBuilder.Parse(query);

		JsonNode node = null;

		try
		{
			using var client = new HttpClient { BaseAddress = new Uri(APIUrl) };
			using HttpRequestMessage request = new();
			request.Method = HttpMethod.Post;

			foreach (var header in request.Headers)
			{
				request.Headers.Add(header.Key, header.Value);
			}

			request.Content = new StringContent(queryString, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.SendAsync(request);
			node = JsonNode.Parse(await response.Content.ReadAsStringAsync());
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return node;
	}

	internal static async Task<JsonNode> GetResponseContent(string query, ILogger logger)
	{
		JsonNode node = null;

		try
		{
			using var client = new HttpClient { BaseAddress = new Uri(APIUrl) };
			using HttpRequestMessage request = new();
			request.Method = HttpMethod.Post;
			request.Headers.Add("Authorization", APISecret);
			request.Headers.Add("API_Version", "2023-10");
			request.Content = new StringContent(query, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				node = JsonNode.Parse(await response.Content.ReadAsStringAsync());
			}
			else
			{
				node = JsonNode.Parse(await response.Content.ReadAsStringAsync());
				throw new MonGraphQLClientException(node.ToString());
			}
		}
		catch (Exception ex)
		{
			string msg = $"GetResponseContent: query: {query}";
			logger.LogError(ex, msg);
			throw new MonGraphQLClientException(ex, msg);
		}

		return node;
	}
}
