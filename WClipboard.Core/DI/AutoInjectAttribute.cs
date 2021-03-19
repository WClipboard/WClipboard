using System;

namespace WClipboard.Core.DI
{
    /// <summary>
    /// Interfaces with this attribute will automaticly be registered refering to the same instance when using the <see cref="WClipboard.Core.DI.Extensions.AddSingletonWithAutoInject"/> extensions. This allows init procedures.
    /// </summary>
    /// <seealso cref="WClipboard.Core.DI.DiContainer.Build"/>
    /// <seealso cref="WClipboard.Core.Boot.IAfterDIContainerBuildListener"/>
    /// <seealso cref="WClipboard.Core.Managers.IOSettingsManager"/>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class AutoInjectAttribute : Attribute
    {
        public AutoInjectAttribute()
        {
        }
    }
}
