namespace MonGraphQLClient;

internal static class QueryBuilder
{
	static QueryBuilder() {  }

    internal static string Parse(Query queryObj)
	{
		StringBuilder sb = new();

		string queryString = string.Empty;

		if (!string.IsNullOrEmpty(queryObj.Prefix))
		{
			sb.Append(queryObj.Prefix);
		}

		if (!string.IsNullOrEmpty(queryObj.OperationType))
		{
			sb.Append(queryObj.OperationType);
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
}
