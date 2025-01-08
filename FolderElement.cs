

namespace CustomLINQProvider;

public class FolderElement(string path) : FileSystemElement(path)
{
    public override ElementType ElementType => ElementType.Folder;
}
