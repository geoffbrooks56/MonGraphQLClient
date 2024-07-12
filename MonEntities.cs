using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;
using System;

namespace MonGraphQLClient;

#region PUBLIC
public class MonBoard
{
    public MonBoard() { }

    public string BoardId { get; set; }

    public string Name { get; set; }

    public List<MonGroup> Groups { get; set; }
}

public class MonGroup
{
    public MonGroup() { }

    public string BoardId { get; set; }

    public string GroupId { get; set; }

    public string Title { get; set; }

    public List<MonItem> Items { get; set; }
}

public class MonItem
{
    public MonItem() { }

    public string ItemId { get; set; }

    public string BoardId { get; set; }

    public string GroupId { get; set; }

    public string GroupTitle { get; set; }

    public string Name { get; set; }

    public string AccountNumber { get; set; }

    public string MainContact { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string LastActivityDate { get; set; }

    public string FollowUpDate { get; set; }

}

#endregion

#region INTERNALS

//public class MonGroupConfigItem
//{
//    public MonGroupConfigItem() { }

//    public string GroupName { get; set; }

//    public string GroupId { get; set; }
//}


internal class RequestHeader
{
	internal RequestHeader() { }

	internal RequestHeader(string name, string value)
	{
		Name = name;
		Value = value;
	}

	internal string Name { get; set; }

	internal string Value { get; set; }
}

internal class Query
{
	internal Query()
	{
		Items = [];
	}

	internal string Prefix { get; set; }
	internal string Suffix { get; set; }
	internal string Body { get; set; }
	internal List<QueryItem> Items { get; set; }
	internal string OperationName { get; set; }
}

internal class QueryItem
{
	internal QueryItem()  { }
	internal string Name { get; set; }
	internal string Value { get; set; }
	internal string Type { get; set; }
	internal string EscapeCharacter { get; set; }
}


#endregion

#region EXCEPTIONS


public class MonGraphQLClientException : Exception
{
	public MonGraphQLClientException(){ }

	public MonGraphQLClientException(string message) : base(message){ }

	public MonGraphQLClientException( Exception inner, string message) : base(message, inner){ }
}


#endregion