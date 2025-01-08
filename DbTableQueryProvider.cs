using System.Linq.Expressions;

namespace CustomLINQProvider;

class DbTableQueryProvider : IQueryProvider
{
    private readonly string connectionString;

    public DbTableQueryProvider(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new DbTable<TElement>(this, expression, connectionString);
    }

    public object? Execute(Expression expression)
    {
        throw new NotImplementedException();
    }

    public TResult Execute<TResult>(Expression expression)
    {
        throw new NotImplementedException();
    }

}
