using System;
using System.Collections;
using System.Collections.Generic;
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

            foreach (var item in items)
            {
                if (item == OriginalValue)
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
}
