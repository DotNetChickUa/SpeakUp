using System.ComponentModel;
using System.Text.Json;
using Dapper;
using Shared;

namespace DatabaseExtensions.Postgres;

using System.Data.Common;
using Npgsql;

[SpeakUpTool]
public class Query
{
	private static DbConnection CreateConnection(string? connectionString)
	{
		return new NpgsqlConnection(connectionString);
	}

    [Description("Executes query on Postgres")]
    public static async Task<object?> ExecuteCommandAsync(string connectionString, string query, CancellationToken cancellationToken)
    {
        var connection = CreateConnection(connectionString);
        var dapperResults = await connection.QueryAsync<dynamic>(query);
        var results = JsonSerializer.Deserialize<IList<object>>(JsonSerializer.Serialize(dapperResults));
        return results;
    }
}