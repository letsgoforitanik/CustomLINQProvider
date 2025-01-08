using System.Linq.Expressions;

namespace CustomLINQProvider;

public class ExpressionTreeModifier : ExpressionVisitor
{
    private IQueryable<FileSystemElement> queryableElements;

    internal ExpressionTreeModifier(IQueryable<FileSystemElement> places)
    {
        this.queryableElements = places;
    }

    protected override Expression VisitConstant(ConstantExpression c)
    {
        if (c.Type == typeof(FileSystemContext))
        {
            return Expression.Constant(queryableElements);
        }
        else
        {
            return c;
        }
    }
}