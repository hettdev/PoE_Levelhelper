using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelHelper.Core.Util
{
    public static class Mathfx
    {
        public static double Sinerp(double start, double end, double value)
        {
            return Lerp(start, end, (double)Math.Sin(value * Math.PI * 0.5f));
        }

        public static double Coserp(double start, double end, double value)
        {
            return Lerp(start, end, 1.0f - (double)Math.Cos(value * Math.PI * 0.5f));
        }

        public static double Lerp(double start, double end, double value)
        {
            return ((1.0f - value) * start) + (value * end);
        }
    }
}
