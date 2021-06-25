using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WClipboard.Core.WPF.Extensions
{
    public static class ColorExtensions
    {
        public static double GetLuminance(this Color color)
        {
            static double f(double c)
            {
                c /= 255;
                c = c <= 0.03928 ?
                    c / 12.92 :
                    Math.Pow((c + 0.055) / 1.055, 2.4);
                return c;
            }

            return 0.2126 * f(color.R) + 0.7152 * f(color.G) + 0.0722 * f(color.B);
        }

        public static double ContrastWith(Color colorA, Color colorB)
        {
            var LA = colorA.GetLuminance();
            var LB = colorB.GetLuminance();

            return LA < LB ? (LB + 0.05) / (LA + 0.05) : (LA + 0.05) / (LB + 0.05);
        }
    }
}
