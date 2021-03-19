using System;
using WClipboard.Core.Settings;

namespace WClipboard.Core.WPF.Settings
{
    public struct VirtualSetting : ISetting, IEquatable<ISetting>
    {
        public string Key { get; }
        public VirtualSetting(string key)
        {
            Key = key;
        }

        public bool Equals(ISetting? other)
        {
            return other?.Key == Key;
        }

        public override bool Equals(object? obj)
        {
            if(obj is ISetting setting)
            {
                return Equals(setting);
            } 
            else if(obj is string key)
            {
                return Key == key;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public static bool operator ==(VirtualSetting? left, VirtualSetting? right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VirtualSetting? left, VirtualSetting? right)
        {
            return !(left == right);
        }
    }
}
