using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Windows.Helpers;

namespace WClipboard.Plugin.ClipboardImplementations.Path
{
    public class SinglePathViewModel : ClipboardImplementationViewModel<PathsImplementation>
    {
        public ConcurrentBindableList<CommandBinding> CommandBindings { get; }
        public PathPart Main { get; }
        public object IconSource { get; }
        public string TypeName { get; }
        public ReadOnlyCollection<PathPart> Parts { get; }

        private bool exists;
        public bool Exists {
            get => exists;
            private set => SetProperty(ref exists, value);
        }

        private PathType type;
        public PathType Type
        {
            get => type;
            private set => SetProperty(ref type, value);
        }

        private string message;
        public string Message
        {
            get => message;
            private set => SetProperty(ref message, value);
        }

        private readonly FileSystemWatcher watcher;

        public SinglePathViewModel(PathsImplementation implementation, ClipboardObjectViewModel clipboardObject) : base(implementation, clipboardObject)
        {
            string fullPath = implementation.Paths[0];

            CommandBindings = new ConcurrentBindableList<CommandBinding>
            {
                new PathPartOpenCommandBinding()
            };

            if (fullPath.EndsWith(System.IO.Path.DirectorySeparatorChar))
            {
                fullPath = fullPath[0..^1];
            }

            Parts = new ReadOnlyCollection<PathPart>(PathPart.CreateFromFullPath(fullPath));

            Main = Parts[^1];

            IconSource = PathInfoHelper.GetIcon(fullPath, IconType.Small);
            TypeName = PathInfoHelper.GetTypeName(fullPath);

            if (Parts.Count > 1) {
                var directory = Parts[^2].FullPath;

                if (Directory.Exists(directory))
                {
                    watcher = new FileSystemWatcher(directory, Main.Name)
                    {
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName,
                        EnableRaisingEvents = true
                    };
                    watcher.Created += Watcher_Created;
                    watcher.Deleted += Watcher_Deleted;
                    watcher.Renamed += Watcher_Renamed;
                }
            } 
            else
            {
                //TODO monitor drive mount
            }

            Type = Main.Name.Contains(".") ? PathType.File : PathType.Directory;
            RecheckExistsAndType();
            if (!Exists)
                Message = $"This {Type.ToString().ToLowerInvariant()} does not exist";
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            RecheckExistsAndType();

            if(!Exists && e.OldFullPath == Main.FullPath)
            {
                Message = $"This {Type.ToString().ToLowerInvariant()} is moved to {e.FullPath}";
            }
        }

        private void RecheckExistsAndType()
        {
            if (File.Exists(Main.FullPath))
            {
                Exists = true;
                Type = PathType.File;
                Message = null;
            } 
            else if(Directory.Exists(Main.FullPath))
            {
                Exists = true;
                Type = PathType.Directory;
                Message = null;
            }
            else
            {
                Exists = false;
            }
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Exists = false;
            Message = $"This {Type.ToString().ToLowerInvariant()} does not exist anymore";
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            RecheckExistsAndType();
        }
    }
}
