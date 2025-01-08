using System.Linq.Expressions;
using System.Text;
using CustomLINQProvider;
using System.Reflection;


public class SqlGenerator : ExpressionVisitor
{
    private readonly StringBuilder sqlPartTwoBuilder;
    private readonly StringBuilder sqlPartOneBuilder;

    public SqlGenerator(Expression expression)
    {
        sqlPartTwoBuilder = new StringBuilder();
        sqlPartOneBuilder = new StringBuilder();

        sqlPartTwoBuilder.Clear();
        sqlPartOneBuilder.Clear();

        Visit(expression);

    }

    public string GenerateSql()
    {
        var sqlPartOne = sqlPartOneBuilder.ToString();
        var sqlPartTwo = sqlPartTwoBuilder.ToString();

        if (sqlPartOne.Length == 0) sqlPartOne = "select * ";
        return sqlPartOne + sqlPartTwo;
    }

    private static string GetSymbol(ExpressionType expressionType)
    {
        return expressionType switch
        {
            ExpressionType.AndAlso => " and ",
            ExpressionType.OrElse => " or ",
            ExpressionType.GreaterThan => " > ",
            ExpressionType.GreaterThanOrEqual => " >= ",
            ExpressionType.LessThan => " < ",
            ExpressionType.LessThanOrEqual => " <= ",
            ExpressionType.Equal => " = ",
            ExpressionType.NotEqual => " <> ",
            _ => throw new NotSupportedException()
        };
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        switch (node.Method.Name)
        {
            case "Where":
                VisitWhereMethodCall(node);
                break;
            case "Select":
                VisitSelectMethodCall(node);
                break;

        }

        return node;
    }

    private void VisitWhereMethodCall(MethodCallExpression node)
    {
        var arg0 = node.Arguments[0];
        var arg1 = node.Arguments[1];

        Visit(arg0);
        sqlPartTwoBuilder.Append(" WHERE ");
        Visit(arg1);
    }

    private void VisitSelectMethodCall(MethodCallExpression node)
    {
        var arg0 = node.Arguments[0];
        var arg1 = node.Arguments[1];

        Visit(arg0);
        sqlPartOneBuilder.Append("SELECT ");
        Visit(arg1);
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {

        if (sqlPartOneBuilder.Length > 0)
        {
            var returnType = node.ReturnType;
            var properties = returnType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propertyNames = properties.Select(p => p.Name).ToArray();
            var commaSeperatedPropertyNames = string.Join(",", propertyNames);
            sqlPartOneBuilder.Append(commaSeperatedPropertyNames);
            return node;
        }

        return base.VisitLambda(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {

        switch (node.NodeType)
        {
            case ExpressionType.AndAlso:
            case ExpressionType.OrElse:
                Visit(node.Left);
                sqlPartTwoBuilder.Append(GetSymbol(node.NodeType));
                Visit(node.Right);
                break;

            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.Equal:
                sqlPartTwoBuilder.Append('(');
                Visit(node.Left);
                sqlPartTwoBuilder.Append(GetSymbol(node.NodeType));
                Visit(node.Right);
                sqlPartTwoBuilder.Append(')');
                break;
        }

        return node;

    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        var nodeType = node.Type;

        if (nodeType.IsGenericType && nodeType.GetGenericTypeDefinition() == typeof(DbTable<>))
        {
            var innerType = nodeType.GetGenericArguments()[0];
            var tableName = innerType.FullName;
            sqlPartTwoBuilder.Append($" from [{tableName}]");
        }
        else if (node.Type == typeof(string))
        {
            sqlPartTwoBuilder.Append($"'{node.Value}'");
        }
        else
        {
            sqlPartTwoBuilder.Append(node.Value);
        }

        return node;

    }

    protected override Expression VisitMember(MemberExpression node)
    {
        sqlPartTwoBuilder.Append(node.Member.Name);
        return node;
    }

}



