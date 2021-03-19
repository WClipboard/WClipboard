using System.Windows.Input;

namespace WClipboard.Plugin.ClipboardImplementations.Path
{
    public class PathPart
    {
        public static RoutedCommand OpenCommand { get; } = new RoutedCommand("Open", typeof(PathPart));

        public string Name { get; }
        public PathPart Parent { get; }
        public PathPart Child { get; private set; }
        public string FullPath { get; }

        public PathPart(string name, PathPart parent)
        {
            Name = name;
            Parent = parent;
            if (Parent != null)
            {
                Parent.Child = this;
                FullPath = Parent.FullPath + System.IO.Path.DirectorySeparatorChar + Name;
            } 
            else
            {
                FullPath = Name;
            }
        }

        public static PathPart[] CreateFromFullPath(string fullPath)
        {
            var parts = fullPath.Split(System.IO.Path.DirectorySeparatorChar);
            var pathParts = new PathPart[parts.Length];
            for(int i = 0; i < parts.Length; i++)
            {
                pathParts[i] = new PathPart(parts[i], i == 0 ? null : pathParts[i - 1]);
            }

            return pathParts;
        }
    }
}
