using System.ComponentModel;
using System.Text.Json;
using Dapper;
using Shared;

namespace DatabaseExtensions.MSSQL;

using System.Data.Common;
using System.Data.SqlClient;

[SpeakUpTool]
public class Query
{
	private static DbConnection CreateConnection(string? connectionString)
	{
		return new SqlConnection(connectionString);
	}

    [Description("Executes query on SQL Server")]
    public static async Task<object?> ExecuteCommandAsync(string connectionString, string query, CancellationToken cancellationToken)
    {
        var connection = CreateConnection(connectionString);
        var dapperResults = await connection.QueryAsync<dynamic>(query);
        var results = JsonSerializer.Deserialize<IList<object>>(JsonSerializer.Serialize(dapperResults));
        return results;
    }
}