using System.Linq.Expressions;
using CustomLINQProvider;

class Program
{
    static void MainOne()
    {
        Expression<Func<User, bool>> expression = user => user.Age > 18 && user.Name == "John";

        var visitor = new SqlGenerator(expression);
        string sql = visitor.GenerateSql();

        Console.WriteLine(sql); // Outputs: ((Age > 18) AND (Name = 'John'))

    }

    static void MainTwo()
    {
        DbTable<User> users = new("");

        var query = users.Where(user => user.Age > 18 && user.Name == "Anik").Select(user => user);
        //var query = users.Where(user => user.Age > 18 || user.Name == "Roger").Select(user => new { user.Name });
        var queryExpression = query.Expression;

        var sqlGenerator = new SqlGenerator(queryExpression);
        string sql = sqlGenerator.GenerateSql();

        Console.WriteLine(sql);
    }

    static void Main()
    {
        var connectionString = "Server=localhost,1433;Database=tempdb;User Id=sa;Password=Hydrogen2@$;TrustServerCertificate=True;";
        var userTable = new DbTable<User>(connectionString);

        var query1 = userTable.Where(user => user.Age > 18).Select(user => new { user.Name });
        var query2 = userTable.Where(user => user.Age > 18).Select(user => user);
        var query3 = userTable.Where(user => user.Age > 18 && user.Name == "Anik Banerjee").Select(user => user);

        var users = query3.ToList();
        
    }
}

