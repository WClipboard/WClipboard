namespace WClipboard.Core.Settings
{
    public interface IResolveableIOSetting : IIOSetting
    {
        public object? GetResolvedValue();
        public void SetResolvedValue(object? value);
    }
}
