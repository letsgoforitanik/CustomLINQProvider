using System.Collections;
using System.Linq.Expressions;

namespace CustomLINQProvider;

public class DbTable<T> : IQueryable<T>
{
    private readonly string connectionString;

    public DbTable(string connectionString)
    {
        this.connectionString = connectionString;
        Expression = Expression.Constant(this);
        Provider = new DbTableQueryProvider(connectionString);
    }

    public DbTable(IQueryProvider provider, Expression expression, string connectionString)
    {
        this.connectionString = connectionString;
        Expression = expression;
        Provider = provider;
    }

    public Type ElementType => throw new NotImplementedException();

    public Expression Expression { get; }

    public IQueryProvider Provider { get; }

    public IEnumerator<T> GetEnumerator()
    {
        var sqlExecutor = new SqlExecutor(connectionString);
        var result = sqlExecutor.ExecuteExpression<T>(Expression);
        return result.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
