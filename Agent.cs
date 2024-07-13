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

	#region PUBLIC ITEM ADDS/EDITS

	public static async Task<string> AddMonGroup(MonGroup group, ILogger logger)
	{
		string newid = "";
		string query = "";

		try
		{
			query = @"{""query"": ""mutation {create_group(board_id: " + group.BoardId +
										@", group_name: \""" + group.Title + @"\"" ) { id} }"" }";

			JsonNode node = await MonService.GetResponseContent(query, logger);

			newid = node["data"]["create_group"]["id"].ToString();

		}
		catch (Exception ex)
		{
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return newid;
	}

	public static async Task<bool> DeleteMonGroup(string boardId, string groupId, ILogger logger)
	{
		bool result;
		string query = "";

		try
		{			
			query = @"{""query"": ""mutation {delete_group(board_id: \""" + boardId + @"\"" , group_id: \""" + groupId + @"\"") { id deleted} } ""}";

			JsonNode node = await MonService.GetResponseContent(query, logger);

			result = (node["data"]["delete_group"]["deleted"].ToString() == "false");

		}
		catch (Exception ex)
		{
			logger.LogError(ex, "query: {query}", query);
			throw new MonGraphQLClientException(ex, $"query: {query}");
		}

		return result;
	}

	//-------------------------------------------------------------------------------------------------------
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

			JsonNode node = await MonService.GetResponseContent(query, logger);

			newid = node["data"]["create_item"]["id"].ToString();

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""account_number\"", value: \""" + groupItem.AccountNumber + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(changequery, logger);

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text9\"", value: \""" + groupItem.MainContact + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(changequery, logger);

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text__1\"", value: \""" + groupItem.Email + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(changequery, logger);

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text0__1\"", value: \""" + groupItem.PhoneNumber + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(changequery, logger);

			changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""date7\"", value: \""" + lastDate + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(changequery, logger);

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

			await MonService.GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text9\"", value: \""" + groupItem.MainContact + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text__1\"", value: \""" + groupItem.Email + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""text0__1\"", value: \""" + groupItem.PhoneNumber + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""date7\"", value: \""" + lastDate + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(query, logger);

			query = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""date72\"", value: \""" + followDate + @"\"" ) { id} }"" }";

			await MonService.GetResponseContent(query, logger);

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

			JsonNode node = await MonService.GetResponseContent(query, logger);

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

	public static async Task<List<MonGroup>> QueryBoardGroups(string boardId, ILogger logger)
	{
		string query = string.Empty;

		logger.LogTrace("in QueryBoardGroups");

		try
		{

			var queryObject = new
			{
				query = @"query{
                        boards (ids: " + boardId + @"){ 
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

		return await GetGroupsForBoard(query, boardId, logger);
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

	

	internal static async Task<List<MonGroup>> GetGroupsForBoard(string query, string boardId, ILogger logger)
	{
		List<MonGroup> groups = [];

		logger.LogTrace("GetGroupsForBoard query: {query}", query);

		try
		{
			JsonNode groupsNode = await MonService.GetResponseContent(query, logger);
			JsonArray groupsArray = groupsNode["data"]["boards"][0]["groups"].AsArray();

			foreach (var groupItem in groupsArray)
			{
				MonGroup group = new()
				{
					BoardId = boardId,
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

			JsonNode node = await MonService.GetResponseContent(query, logger);

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

}
