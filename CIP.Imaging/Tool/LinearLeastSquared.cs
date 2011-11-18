using System;
using System.Collections.Generic;

namespace Cip.Imaging.Tool
{
    public class LinearLeastSquared
    {
        /// <summary>
        /// Gives best fit of data to line y = kx + c
        /// </summary>
        /// <param name="points"></param>
        /// <param name="numPoints"></param>
        /// <param name="k"></param>
        /// <param name="c"></param>
        public static void Calculate(IEnumerable<System.Drawing.PointF> points, ref double k, ref double c)
        {
            double x1 = 0;
            double y1 = 0;
            double xy = 0;
            double x2 = 0;
            double numberOfPoints = 0;

            foreach (var p in points)
            {
                x1 += p.X;
                y1 += p.Y;
                xy += p.X * p.Y;
                x2 += p.X * p.X;
                numberOfPoints++;
            }

            var J = (numberOfPoints * x2) - (x1 * x1);
            if (J != 0.0)
            {
                k = ((numberOfPoints * xy) - (x1 * y1)) / J;
                k = Math.Floor(1000.0 * k + 0.5) / 1000.0;
                c = ((y1 * x2) - (x1 * xy)) / J;
                c = Math.Floor(1000.0 * c + 0.5) / 1000.0;
            }
            else
            {
                k = 0;
                c = 0;
            }
        }
    }
}
