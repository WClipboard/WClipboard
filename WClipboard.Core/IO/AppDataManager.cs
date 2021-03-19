using System;
using System.IO;

namespace WClipboard.Core.IO
{
    public interface IAppDataManager
    {
        string RoamingPath { get; } 
    }

    public class AppDataManager : IAppDataManager
    {
        public string RoamingPath { get; }

        public AppDataManager(IAppInfo appInfo)
        {
            RoamingPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\{appInfo.Name}\";
            if(!Directory.Exists(RoamingPath))
            {
                Directory.CreateDirectory(RoamingPath);
            }
        }
    }
}
