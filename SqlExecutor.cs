using System.Linq.Expressions;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CustomLINQProvider;

public class SqlExecutor
{
    private readonly string connectionString;

    public SqlExecutor(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public IEnumerable<T> ExecuteExpression<T>(Expression expression)
    {
        var sqlGenerator = new SqlGenerator(expression);
        var sql = sqlGenerator.GenerateSql();

        using var connection = new SqlConnection(connectionString);
        return connection.Query<T>(sql);
    }
}
