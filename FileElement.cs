namespace CustomLINQProvider;

public class FileElement(string path) : FileSystemElement(path)
{
    public override ElementType ElementType => ElementType.File;
}
