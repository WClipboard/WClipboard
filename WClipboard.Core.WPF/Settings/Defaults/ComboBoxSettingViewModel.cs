using System;
using System.Collections;
using System.Collections.Generic;
using WClipboard.Core.Extensions;
using WClipboard.Core.Settings;

namespace WClipboard.Core.WPF.Settings.Defaults
{
    public class ComboBoxSettingViewModel : SettingViewModel
    {
        public IEnumerable Items { get; }

        public object? ItemTemplateViewOptions { get; }

        public ComboBoxSettingViewModel(
            ISetting model, 
            ISettingApplier settingsApplier, 
            IEnumerable items, 
            string description, 
            object? itemTemplateViewOptions = null) : base(model, settingsApplier, description)
        {
            Items = items;
            ItemTemplateViewOptions = itemTemplateViewOptions;

            var org = OriginalValue;

            foreach (var item in items)
            {
                if (item == OriginalValue || item.Equals(OriginalValue))
                    return;
            }

            throw new ArgumentException("The start value must also be inside the provided items", nameof(items));
        }
    }

    public class ComboBoxSettingViewModel<TItem> : ComboBoxSettingViewModel
    {
        public new TItem Value
        {
            get => (TItem)base.Value!;
            set => base.Value = value;
        }
        public new IEnumerable<TItem> Items => (IEnumerable<TItem>)base.Items;

        public ComboBoxSettingViewModel(
            ISetting model, 
            ISettingApplier<TItem> settingsApplier, 
            IEnumerable<TItem> items, 
            string description, 
            object? itemTemplateViewOptions = null) : base(model, settingsApplier, items, description, itemTemplateViewOptions)
        {
        }
    }

    public class ComboBoxSettingEnumViewModel<TEnum> : ComboBoxSettingViewModel<TEnum> where TEnum : Enum
    {
        public ComboBoxSettingEnumViewModel(ISetting model, ISettingApplier<TEnum> settingsApplier, string description, object? itemTemplateViewOptions = null) 
            : base(model, settingsApplier, EnumExtensions.GetValues<TEnum>(), description, itemTemplateViewOptions)
        {
        }
    }
}
