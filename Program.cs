using CustomLINQProvider;

/*var query = from element in new FileSystemContext(@"C:\Downloads")
            where element.ElementType == ElementType.File && element.Path.EndsWith(".zip")
            orderby element.Path ascending
            select element;*/


var fileSystem = new FileSystemContext(@"/home/anik/Downloads/DOTNET");
var query = fileSystem.Where(element => element.ElementType == ElementType.File);
var result = query.ToList();

foreach (var item in result)
{
    Console.WriteLine(item.Path);
}

