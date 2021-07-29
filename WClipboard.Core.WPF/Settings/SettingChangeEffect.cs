namespace WClipboard.Core.WPF.Settings
{
    /// <summary>
    /// Display to the user what they have to do to make the changed settings become effective
    /// </summary>
    public enum SettingChangeEffect
    {
        AtOnce,
        ReloadRequired,
        RestartRequired
    }
}
