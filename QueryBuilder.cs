using Microsoft.Extensions.Primitives;

namespace MonGraphQLClient;

public static class QueryBuilder
{
	static QueryBuilder() {  }

	public static string Parse(Query query)
	{
		StringBuilder sb = new();

		string qs = string.Empty;

		sb.Append("{");
		sb.Append(@"""query"":");
		 
		if (query.OperationType == OperationTypeStrings.Query)
		{
			sb.Append(@"""query ");
		}
		else
		{
			sb.Append(@"""mutation ");
		}

		if (query.Argument is not null)
		{
			sb.Append(@$"{query.Argument.Name} ( ");

			foreach (var argVar in query.Argument.Variables)
			{
				sb.Append($"{argVar.Name}: ");

				switch (argVar.Type)
				{


				}
			}

			sb.Append(" )");
		}

		/*
		{
			"query": "query {boards (ids: 6422224156) {groups {title id}}}" 
		}
		 */

		sb.Append(@"""}");

		return qs;
	}
}
