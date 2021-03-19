using System;
using System.IO;

namespace WClipboard.Core.IO
{
    public interface ITempManager
    {
        string GetNewFileName(string extension);
    }

    public class TempManager : ITempManager
    {
        private readonly string directoryName;

        public TempManager(IAppInfo appInfo)
        {
            var sep = Path.DirectorySeparatorChar;
            directoryName = $"{Path.GetTempPath()}{appInfo.Name}{sep}{DateTime.Now:yyMMddHHmmss}{sep}";
            Directory.CreateDirectory(directoryName);
        }

        public string GetNewFileName(string extension)
        {
            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            return $"{directoryName}{Guid.NewGuid().ToString().Replace("-", "")}.{extension}";
        }
    }
}
