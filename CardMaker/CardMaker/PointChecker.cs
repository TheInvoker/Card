using System;
using System.Drawing;

namespace CardMaker
{
    class PointChecker
    {
        // http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle

        public static Boolean PointInQuad(Point p, Point p0, Point p1, Point p2, Point p3)
        {
            return PointInTriangle(p, p0, p1, p2) || PointInTriangle(p, p0, p2, p3);
        }

        private static Boolean PointInTriangle(Point p, Point p0, Point p1, Point p2)
        {
            var s = p0.Y * p2.X - p0.X * p2.Y + (p2.Y - p0.Y) * p.X + (p0.X - p2.X) * p.Y;
            var t = p0.X * p1.Y - p0.Y * p1.X + (p0.Y - p1.Y) * p.X + (p1.X - p0.X) * p.Y;

            if ((s < 0) != (t < 0))
                return false;

            var A = -p1.Y * p2.X + p0.Y * (p2.X - p1.X) + p0.X * (p1.Y - p2.Y) + p1.X * p2.Y;
            if (A < 0.0)
            {
                s = -s;
                t = -t;
                A = -A;
            }
            return s > 0 && t > 0 && (s + t) < A;
        }
    }
}


