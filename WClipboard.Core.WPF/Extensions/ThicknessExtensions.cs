using System.Windows;

namespace WClipboard.Core.WPF.Extensions
{
    public static class ThicknessExtensions
    {
        public static Thickness Add(this Thickness item1, Thickness item2)
        {
            return new Thickness(
                item1.Left + item2.Left,
                item1.Top + item2.Top,
                item1.Right + item2.Right,
                item1.Bottom + item2.Bottom);
        }

        public static Thickness Sub(this Thickness item1, Thickness item2)
        {
            return new Thickness(
                item1.Left - item2.Left,
                item1.Top - item2.Top,
                item1.Right - item2.Right,
                item1.Bottom - item2.Bottom);
        }
    }
}
