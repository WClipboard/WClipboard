using System;
using System.Collections;
using System.Globalization;

namespace WClipboard.Core.WPF.Converters
{
    public class IndexConverter : BaseConverter<IEnumerable, object?>
    {
        public int Index { get; set; }

        public override object? Convert(IEnumerable value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!(parameter is int index))
                index = Index;


            if(value is IList list)
            {
                if (index < 0)
                    index += list.Count;

                return list[index];
            } 
            else
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} can not be below 0 if the target is a {nameof(IEnumerable)}");

                var enumerator = value.GetEnumerator();
                int i = 0;
                for(; i <= index && enumerator.MoveNext(); i++) { }
                return enumerator.Current ?? throw new IndexOutOfRangeException($"Index: {index} is not in set with size: {i}");
            }
        }
    }
}
