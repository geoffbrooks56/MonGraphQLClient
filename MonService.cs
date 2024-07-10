using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MonGraphQLClient;

public static class MonService
{
    static string mondayAPIAuth;
    static string mondayAPIUrl;

    static MonService()
    {
        mondayAPIAuth = Environment.GetEnvironmentVariable("MondayAPIToken", EnvironmentVariableTarget.Process);
        mondayAPIUrl = Environment.GetEnvironmentVariable("MondayURL", EnvironmentVariableTarget.Process);
    }

    #region Configure

   

    #endregion

    #region Get Monday Entities

    public static List<MonBoard> GetBoards(ILogger logger)
    {
        logger.LogTrace("in GetBoards");

        List<MonBoard> boards =
        [
            new() {BoardId = "6422224156", Name = "OPEN QUOTES FRAMEWORK"}
        ];

        return boards;
    }

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
            throw;
        }

        return await FetchBoardGroups(query, board, logger);
    }

    public static async Task<List<MonGroupItem>> QueryBoardGroupItems(MonGroup group, ILogger logger)
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
            throw;
        }

        return await FetchBoardGroupItems(query, group, logger);
    }

    static async Task<List<MonGroup>> FetchBoardGroups(string query, MonBoard board, ILogger logger)
    {
        List<MonGroup> groups = [];

        logger.LogTrace("FetchBoardGroups query: {query}", query);

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
            throw;
        }

        return groups;
    }

    static async Task<List<MonGroupItem>> FetchBoardGroupItems(string query, MonGroup group, ILogger logger)
    {
        List<MonGroupItem> groupItems = [];

        try
        {
            logger.LogTrace("FetchBoardGroupItems query: {query}", query);

            JsonNode node = await GetResponseContent(query, logger);

            string groupjson = node["data"]["boards"][0]["groups"].ToJsonString();
            if (groupjson != "[]")
            {
                JsonNode itemspagenode = JsonNode.Parse(groupjson);
                JsonArray itemsArray = itemspagenode[0].AsObject()["items_page"].AsObject()["items"].AsArray();

                foreach (var item in itemsArray)
                {
                    MonGroupItem mgItem = new()
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
            throw;
        }

        return groupItems;
    }

    #endregion

    #region Monday Item Edits

    public static async Task<string> AddMondayItem(MonGroupItem groupItem, ILogger logger)
    {
        string newid = "";

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

            string query = @"{""query"": ""mutation {create_item(board_id: " + groupItem.BoardId +
                                        @", group_id: \""" + groupItem.GroupId + @"\"" item_name: \""" + groupItem.Name + @"\"" ) { id} }"" }";

            JsonNode node = await GetResponseContent(query, logger);

            newid = node["data"]["create_item"]["id"].ToString();

            string changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""account_number\"", value: \""" + groupItem.AccountNumber + @"\"" ) { id} }"" }";

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
            logger.LogError(ex, "UpdateMondayFromPrintavo");
            throw;
        }

        return newid;
    }

    public static async Task<bool> UpdateMondayItem(MonGroupItem groupItem, ILogger logger)
    {

        string newid = groupItem.ItemId;

        DateTime lastActivityDate = Convert.ToDateTime(groupItem.LastActivityDate);
        string lastDate = lastActivityDate.ToString("yyyy-MM-dd");

        DateTime followupDate = Convert.ToDateTime(groupItem.FollowUpDate);
        if (followupDate == DateTime.MinValue)
        {
            followupDate = DateTime.Now.AddDays(14);
        }
        string followDate = followupDate.ToString("yyyy-MM-dd");

        string changequery = @"{""query"": ""mutation {change_simple_column_value (item_id: " + newid + @", board_id: " + groupItem.BoardId + @", column_id: \""account_number\"", value: \""" + groupItem.AccountNumber + @"\"" ) { id} }"" }";

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

        await GetResponseContent(changequery, logger);

        return true;
    }

    public static async Task<string> MoveMondayItem(string itemId, string newGroupId, ILogger logger)
    {
        string query = @"{""query"": ""mutation {move_item_to_group(item_id: " + itemId + @", group_id: \""" + newGroupId + @"\""){id}}""}";

        JsonNode node = await GetResponseContent(query, logger);

        string newid = node["data"]["move_item_to_group"]["id"].ToString();

        return newid;
    }

    #endregion

    static async Task<JsonNode> GetResponseContent(string query, ILogger logger)
    {   
        JsonNode node = null;

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(mondayAPIUrl) };
            using HttpRequestMessage request = new();
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", mondayAPIAuth);
            request.Headers.Add("API_Version", "2023-10");
            request.Content = new StringContent(query, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            node = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetResponseContent");
            throw;
        }

        return node;
    }
}
