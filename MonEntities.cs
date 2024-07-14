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

public class Query
{
	public Query()
	{
		Inputs = [];
		Outputs = [];
	}

	public string ObjectType { get; set; }
	public Argument Argument { get; set; }
	public string Suffix { get; set; }
	public string Body { get; set; }
	public List<Variable> Inputs { get; set; }
	public List<Variable> Outputs { get; set; }
	public string OperationType { get; set; }
}

public struct OperationTypeStrings
{
	public const string Query = "query";
	public const string Mutation = "mutation";
}

public struct VariableTypeStrings
{
	public const string String = "String";
	public const string Int = "Int";
	public const string Boolean = "Boolean";
	public const string Object = "OBJECT";
	public const string ID = "ID";
	public const string Date = "Date";
}

public class Variable
{
	public Variable()  { }
	public string Name { get; set; }
	public string Value { get; set; }
	public string Type { get; set; }
	public List<Variable> SubVariables { get; set; }
}

public class Argument
{
	public Argument() 
	{
		Variables = [];
	}
	public string Name { get; set; }
	public List<Variable> Variables { get; set; }
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