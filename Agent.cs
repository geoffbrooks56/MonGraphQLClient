using System.Text.Json.Nodes;

namespace MonGraphQLClient;

public class Agent
{
    public string APIUrl { get; set; }
    public string APISecret { get; set; }

    public List<RequestHeader> RequestHeaders { get; set; }

    public Agent(){ }

    public Agent(string apiUrl, string apiKey)
    {
        APIUrl = apiUrl;
        APISecret = apiKey;
    }

    public async Task<JsonNode> GetResultAsync(Query query)
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
            Console.Write(ex.ToString());
            throw;
        }

        return node;
    }
    
    public class RequestHeader
    {
        public RequestHeader() {}

        public RequestHeader(string name, string value)
        {
            Name = name; 
            Value = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }
    }

    static string ConstructQueryString(Query queryObj)
    {
        StringBuilder sb = new ();

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

        bool hasVars = (queryObj.Variables.Count > 0);

        if (hasVars) sb.Append('{');

        foreach (var qvar in  queryObj.Variables)
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

    public class Query
    {
        public Query() 
        {
            Variables = [];
        }

        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string Body { get; set; }
        public List<Variable> Variables { get; set; }
        public string OperationName { get; set; }   
    }

    public class Variable
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string EscapeCharacter { get; set; }
    }
}
