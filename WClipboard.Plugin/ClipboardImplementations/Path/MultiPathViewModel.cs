using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;
using WClipboard.Plugin.ClipboardImplementations.Path.Interactables;
using WClipboard.Windows.Helpers;

namespace WClipboard.Plugin.ClipboardImplementations.Path
{
    public class MultiPathViewModel : BindableBase, IHasAssignableInteractables
    {
        public BindableObservableCollection<CommandBinding> CommandBindings { get; }
        public ObservableCollection<InteractableState> Interactables { get; }
        IReadOnlyList<InteractableState> IHasInteractables.Interactables => Interactables;
        public PathPart Main { get; }
        public object IconSource { get; }
        public string TypeName { get; }
        public ReadOnlyCollection<PathPart> Parts { get; }

        public MultiPathViewModel(string fullPath, SynchronizationContext synchronizationContext)
        {
            CommandBindings = new BindableObservableCollection<CommandBinding>(synchronizationContext)
            {
                new PathPartOpenCommandBinding()
            };

            Interactables = new BindableObservableCollection<InteractableState>(synchronizationContext)
            {
                new CopyPathPartInteractable().CreateState(this)
            };


            if (fullPath.EndsWith(System.IO.Path.DirectorySeparatorChar))
            {
                fullPath = fullPath[0..^1];
            }

            Parts = new ReadOnlyCollection<PathPart>(PathPart.CreateFromFullPath(fullPath));

            Main = Parts[^1];

            IconSource = PathInfoHelper.GetIcon(fullPath, IconType.Small);
            TypeName = PathInfoHelper.GetTypeName(fullPath);
        }
    }
}
