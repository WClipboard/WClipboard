using System.Collections.ObjectModel;
using System.Linq;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel;

namespace WClipboard.Plugin.ClipboardImplementations.Path
{
    public class MultiPathsViewModel : ClipboardImplementationViewModel<PathsImplementation>
    {
        public ReadOnlyCollection<MultiPathViewModel> PathViewModels { get; }
        public MultiPathViewModel SharedPath { get; }

        public MultiPathsViewModel(PathsImplementation implementation, ClipboardObjectViewModel clipboardObject) : base(implementation, clipboardObject)
        {
            var pathViewModels = new MultiPathViewModel[implementation.Paths.Count];
            for(int i = 0; i < implementation.Paths.Count; i++)
            {
                pathViewModels[i] = new MultiPathViewModel(implementation.Paths[i]);
            }
            PathViewModels = new ReadOnlyCollection<MultiPathViewModel>(pathViewModels);

            //determine shared path
            var firstPathViewModelParts = PathViewModels[0].Parts;
            int lowestSubPathCount = PathViewModels.Min(pvm => pvm.Parts.Count);
            int currentSubPathIndex = 0;
            for(; currentSubPathIndex < lowestSubPathCount; currentSubPathIndex++)
            {
                var currentSubPathName = firstPathViewModelParts[currentSubPathIndex].Name;
                if (!PathViewModels.Select(pvm => pvm.Parts[currentSubPathIndex]).All(pp => pp.Name == currentSubPathName))
                {
                    break;
                }
            }

            if(currentSubPathIndex > 0)
            {
                SharedPath = new MultiPathViewModel(firstPathViewModelParts[currentSubPathIndex -1].FullPath);
            }
        }
    }
}
