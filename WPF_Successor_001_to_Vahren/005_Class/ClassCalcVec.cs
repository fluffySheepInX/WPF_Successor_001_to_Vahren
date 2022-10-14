using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public static class ClassCalcVec
    {
        public static Point ReturnVecDistance(Point from, Point to)
        {
            Point re = new Point();
            re.X = to.X - from.X;
            re.Y = to.Y - from.Y;
            return re;
        }

        public static Point ReturnNormalize(Point target)
        {
            var result = target;
            var calc = Math.Sqrt(target.X * target.X + target.Y * target.Y);
            if (calc > 0)
            {
                var reciprocal = 1 / calc;
                result.X *= reciprocal;
                result.Y *= reciprocal;
            }
            return result;
        }

    }
}
