using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MonGraphQLClient;

public static class Agent
{
	static string APIUrl { get; set; }
	static string APISecret { get; set; }
	static string MasterBoardId { get; set; }

	static Agent()
	{
		APISecret = Environment.GetEnvironmentVariable("MondayAPIToken", EnvironmentVariableTarget.Process);
		APIUrl = Environment.GetEnvironmentVariable("MondayURL", EnvironmentVariableTarget.Process);
		MasterBoardId = Environment.GetEnvironmentVariable("MondayBoardId", EnvironmentVariableTarget.Process);
	}

	#region PUBLIC ITEM EDITS

	public static async Task<string> AddMonItem(MonItem groupItem, ILogger logger)
	{
		string newid = "";
		string query = "";
		string changequery = "";

		try
		{
			DateTime lastActivityDate = Convert.ToDateTime(groupItem.LastActivityDate);
			string lastDate = lastActivityDate.ToString("yyyy-MM-dd");

			DateTime followupDate = Convert.ToDateTime(groupItem.FollowUpDate);
			if (followupDate == DateTime.MinValue)
			{
				followupDate = DateTime.Now.AddDays(14);
			}
			string followDate = followupDate.ToString("yyyy-MM-dd");

			query = @"{""query"": ""mutation {create_item(board_id: " + groupItem.BoardId +
										@", group_id: \""" + groupItem.GroupId + @"\"" item_name: \""" + groupItem.Name + @"\"" ) { id} }"" }";

			JsonNode node = await GetResponseContent(query, logger);

			newid = node["data"]["create_item"]["id"].ToString();

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""account_number\"", value: \""" + groupItem.AccountNumber + @"\"" ) { id} }"" }";

			await GetResponseContent(changequery, logger);

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text9\"", value: \""" + groupItem.MainContact + @"\"" ) { id} }"" }";

			await GetResponseContent(changequery, logger);

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text__1\"", value: \""" + groupItem.Email + @"\"" ) { id} }"" }";

			await GetResponseContent(changequery, logger);

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text0__1\"", value: \""" + groupItem.PhoneNumber + @"\"" ) { id} }"" }";

			await GetResponseContent(changequery, logger);

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""date7\"", value: \""" + lastDate + @"\"" ) { id} }"" }";

			await GetResponseContent(changequery, logger);

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""date72\"", value: \""" + followDate + @"\"" ) { id} }"" }";

		}
		catch (Exception ex)
		{
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return newid;
	}

	public static async Task<bool> UpdateMonItem(MonItem groupItem, ILogger logger)
	{

		string newid = groupItem.ItemId;
		string query = "";

		try
		{
			DateTime lastActivityDate = Convert.ToDateTime(groupItem.LastActivityDate);
			string lastDate = lastActivityDate.ToString("yyyy-MM-dd");

			DateTime followupDate = Convert.ToDateTime(groupItem.FollowUpDate);
			if (followupDate == DateTime.MinValue)
			{
				followupDate = DateTime.Now.AddDays(14);
			}
			string followDate = followupDate.ToString("yyyy-MM-dd");

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""account_number\"", value: \""" + groupItem.AccountNumber + @"\"" ) { id} }"" }";

			await GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text9\"", value: \""" + groupItem.MainContact + @"\"" ) { id} }"" }";

			await GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text__1\"", value: \""" + groupItem.Email + @"\"" ) { id} }"" }";

			await GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text0__1\"", value: \""" + groupItem.PhoneNumber + @"\"" ) { id} }"" }";

			await GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""date7\"", value: \""" + lastDate + @"\"" ) { id} }"" }";

			await GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""date72\"", value: \""" + followDate + @"\"" ) { id} }"" }";

			await GetResponseContent(query, logger);

		}
		catch (Exception ex)
		{
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return true;
	}

	public static async Task<string> MoveMonItem(string itemId, string newGroupId, ILogger logger)
	{
		string query = "";
		string newid = "";

		try
		{
			query = @"{""query"": ""mutation {move_item_to_group(item_id: " + itemId + @", group_id: \""" + newGroupId + @"\""){id}}""}";

			JsonNode node = await GetResponseContent(query, logger);

			newid = node["data"]["move_item_to_group"]["id"].ToString();
		}

		catch (Exception ex)
		{
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return newid;
	}

	#endregion

	#region PUBLIC QUERY BOARDS AND GROUPS

	public static async Task<List<MonGroup>> QueryBoardGroups(MonBoard board, ILogger logger)
	{
		string query = string.Empty;

		logger.LogTrace("in QueryBoardGroups");

		try
		{

			var queryObject = new
			{
				query = @"query{
                        boards (ids: " + board.BoardId + @"){ 
                            groups{
                                id
                                title
                            }
                        }
                      }"
			};

			query = JsonSerializer.Serialize(queryObject);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return await GetGroupsForBoard(query, board, logger);
	}

	public static async Task<List<MonItem>> QueryBoardGroupItems(MonGroup group, ILogger logger)
	{
		string query = string.Empty;

		logger.LogTrace("in QueryBoardGroupItems");

		try
		{
			var queryObject = new
			{
				query = @"query{
                        boards (ids: " + group.BoardId + @"){
                            groups (ids: """ + group.GroupId + @"""){
                                items_page (limit: 100){       
                                    cursor
                                    items{
                                        id 
                                        name 
                                        column_values{
                                            id 
                                            type
                                            text 
                                            column{
                                                title                
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }"
			};

			query = JsonSerializer.Serialize(queryObject);

		}
		catch (Exception ex)
		{
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return await GetItemsForBoard(query, group, logger);
	}

	#endregion

	#region INTERNALS
	internal static List<RequestHeader> RequestHeaders { get; set; }

	internal static async Task<JsonNode> GetResultAsync(Query query, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(nameof(query));

		string queryString = ConstructQueryString(query);

		JsonNode node = null;

		try
		{
			using var client = new HttpClient { BaseAddress = new Uri(APIUrl) };
			using HttpRequestMessage request = new();
			request.Method = HttpMethod.Post;

			foreach (var header in RequestHeaders)
			{
				request.Headers.Add(header.Name, header.Value);
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

	internal static async Task<List<MonGroup>> GetGroupsForBoard(string query, MonBoard board, ILogger logger)
	{
		List<MonGroup> groups = [];

		logger.LogTrace("GetGroupsForBoard query: {query}", query);

		try
		{
			JsonNode groupsNode = await GetResponseContent(query, logger);
			JsonArray groupsArray = groupsNode["data"]["boards"][0]["groups"].AsArray();

			foreach (var groupItem in groupsArray)
			{
				MonGroup group = new()
				{
					BoardId = board.BoardId,
					GroupId = groupItem["id"].ToString(),
					Title = groupItem["title"].ToString()
				};
				groups.Add(group);
			}
		}
		catch (Exception ex)
		{			
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return groups;
	}

	internal static async Task<List<MonItem>> GetItemsForBoard(string query, MonGroup group, ILogger logger)
	{
		List<MonItem> groupItems = [];

		try
		{
			logger.LogTrace("GetItemsForBoard query: {query}", query);

			JsonNode node = await GetResponseContent(query, logger);

			string groupjson = node["data"]["boards"][0]["groups"].ToJsonString();
			if (groupjson != "[]")
			{
				JsonNode itemspagenode = JsonNode.Parse(groupjson);
				JsonArray itemsArray = itemspagenode[0].AsObject()["items_page"].AsObject()["items"].AsArray();

				foreach (var item in itemsArray)
				{
					MonItem mgItem = new()
					{
						ItemId = item["id"].ToString(),
						BoardId = group.BoardId,
						GroupId = group.GroupId,
						GroupTitle = group.Title,
						Name = item["name"].ToString()
					};

					JsonArray columnValues = item["column_values"].AsArray();
					foreach (var columnValue in columnValues)
					{
						switch (columnValue["id"].ToString())
						{
							case "account_number":
								mgItem.AccountNumber = columnValue["text"].ToString();
								break;
							case "text9":
								mgItem.MainContact = columnValue["text"].ToString();
								break;
							case "date7":
								mgItem.LastActivityDate = columnValue["text"].ToString();
								break;
							case "date72":
								mgItem.FollowUpDate = columnValue["text"].ToString();
								break;
							case "text__1":
								mgItem.Email = columnValue["text"].ToString();
								break;
							case "text0__1":
								mgItem.PhoneNumber = columnValue["text"].ToString();
								break;
						}
					}

					groupItems.Add(mgItem);
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return groupItems;
	}

	#endregion

	#region PRIVATES

	static string ConstructQueryString(Query queryObj)
	{
		StringBuilder sb = new();

		string queryString = string.Empty;

		if (!string.IsNullOrEmpty(queryObj.Prefix))
		{
			sb.Append(queryObj.Prefix);
		}

		if (!string.IsNullOrEmpty(queryObj.OperationName))
		{
			sb.Append(queryObj.OperationName);
		}

		if (!string.IsNullOrEmpty(queryObj.Body))
		{
			sb.Append(queryObj.Body);
		}

		bool hasVars = (queryObj.Items.Count > 0);

		if (hasVars) sb.Append('{');

		foreach (var qvar in queryObj.Items)
		{
			bool hasEsc = !string.IsNullOrEmpty(qvar.EscapeCharacter);

			if (hasEsc)
			{
				if (hasEsc) sb.Append(qvar.EscapeCharacter);
				sb.Append(qvar.Name);
				if (hasEsc) sb.Append(qvar.EscapeCharacter);
				if (hasEsc) sb.Append(qvar.EscapeCharacter);
				sb.Append(qvar.Value);
				if (hasEsc) sb.Append(qvar.EscapeCharacter);
				sb.Append(':');
			}
		}

		if (hasVars) sb.Append('}');

		if (!string.IsNullOrEmpty(queryObj.Suffix))
		{
			sb.Append(queryObj.Suffix);
		}

		return queryString;
	}

	static async Task<JsonNode> GetResponseContent(string query, ILogger logger)
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
			node = JsonNode.Parse(await response.Content.ReadAsStringAsync());
		}
		catch (Exception ex)
		{
			string msg = $"GetResponseContent: query: {query}";
			logger.LogError(ex, msg);
			throw new MonGraphQLClientException(ex, msg);
		}

		return node;
	}
	
	#endregion
}
