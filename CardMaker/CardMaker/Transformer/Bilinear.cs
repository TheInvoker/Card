using System.Collections.Generic;
using System.Drawing;
using System;

namespace CardMaker
{
    class Bilinear : Transformer
    {
        public override void DrawShape(int w, int h, Shape original, Shape warped, Dictionary<Point, Point> mapping)
        {
            if (!original.GetTopLeftPixel().GetColor().Equals(Color.FromArgb(40,40,120)))
            {
                //return;
            }

            double[,] X = new double[4, 4];
            double[] Y = new double[4];

            X[0, 0] = 1; X[1, 0] = original.GetTopLeftPixel().GetX(); X[2, 0] = original.GetTopLeftPixel().GetY(); X[3, 0] = X[1, 0] * X[2, 0];
            X[0, 1] = 1; X[1, 1] = original.GetTopRightPixel().GetX(); X[2, 1] = original.GetTopRightPixel().GetY(); X[3, 1] = X[1, 1] * X[2, 1];
            X[0, 2] = 1; X[1, 2] = original.GetBottomRightPixel().GetX(); X[2, 2] = original.GetBottomRightPixel().GetY(); X[3, 2] = X[1, 2] * X[2, 2];
            X[0, 3] = 1; X[1, 3] = original.GetBottomLeftPixel().GetX(); X[2, 3] = original.GetBottomLeftPixel().GetY(); X[3, 3] = X[1, 3] * X[2, 3];

            Y[0] = warped.GetTopLeftPixel().GetX();
            Y[1] = warped.GetTopRightPixel().GetX();
            Y[2] = warped.GetBottomRightPixel().GetX();
            Y[3] = warped.GetBottomLeftPixel().GetX();


            Console.WriteLine("[{0} {1} {2} {3}][a0] [{4}]", X[0, 0], X[1, 0], X[2, 0], X[3, 0], Y[0]);
            Console.WriteLine("[{0} {1} {2} {3}][a1]=[{4}]", X[0, 1], X[1, 1], X[2, 1], X[3, 1], Y[1]);
            Console.WriteLine("[{0} {1} {2} {3}][a2] [{4}]", X[0, 2], X[1, 2], X[2, 2], X[3, 2], Y[2]);
            Console.WriteLine("[{0} {1} {2} {3}][a3] [{4}]", X[0, 3], X[1, 3], X[2, 3], X[3, 3], Y[3]);
            Solver.Solve(X, Y);
            Console.WriteLine("coefficients: {0} {1} {2} {3}", Y[0], Y[1], Y[2], Y[3]);
            Console.WriteLine(X[0, 0] * Y[0] + X[1, 0] * Y[1] + X[2, 0] * Y[2] + X[3, 0] * Y[3]);
            Console.WriteLine(X[0, 1] * Y[0] + X[1, 1] * Y[1] + X[2, 1] * Y[2] + X[3, 1] * Y[3]);
            Console.WriteLine(X[0, 2] * Y[0] + X[1, 2] * Y[1] + X[2, 2] * Y[2] + X[3, 2] * Y[3]);
            Console.WriteLine(X[0, 3] * Y[0] + X[1, 3] * Y[1] + X[2, 3] * Y[2] + X[3, 3] * Y[3]);

            double a0 = Y[0], a1 = Y[1], a2 = Y[2], a3 = Y[3];

            Y[0] = warped.GetTopLeftPixel().GetY();
            Y[1] = warped.GetTopRightPixel().GetY();
            Y[2] = warped.GetBottomRightPixel().GetY();
            Y[3] = warped.GetBottomLeftPixel().GetY();

            Console.WriteLine("[{0} {1} {2} {3}][a0] [{4}]", X[0, 0], X[1, 0], X[2, 0], X[3, 0], Y[0]);
            Console.WriteLine("[{0} {1} {2} {3}][a1]=[{4}]", X[0, 1], X[1, 1], X[2, 1], X[3, 1], Y[1]);
            Console.WriteLine("[{0} {1} {2} {3}][a2] [{4}]", X[0, 2], X[1, 2], X[2, 2], X[3, 2], Y[2]);
            Console.WriteLine("[{0} {1} {2} {3}][a3] [{4}]", X[0, 3], X[1, 3], X[2, 3], X[3, 3], Y[3]);
            Solver.Solve(X, Y);
            Console.WriteLine("coefficients: {0} {1} {2} {3}", Y[0], Y[1], Y[2], Y[3]);
            Console.WriteLine(X[0, 0] * Y[0] + X[1, 0] * Y[1] + X[2, 0] * Y[2] + X[3, 0] * Y[3]);
            Console.WriteLine(X[0, 1] * Y[0] + X[1, 1] * Y[1] + X[2, 1] * Y[2] + X[3, 1] * Y[3]);
            Console.WriteLine(X[0, 2] * Y[0] + X[1, 2] * Y[1] + X[2, 2] * Y[2] + X[3, 2] * Y[3]);
            Console.WriteLine(X[0, 3] * Y[0] + X[1, 3] * Y[1] + X[2, 3] * Y[2] + X[3, 3] * Y[3]);

            double b0 = Y[0], b1 = Y[1], b2 = Y[2], b3 = Y[3];


            double A = (b2 * a3) - (b3 * a2);
            if (A == 0)
                A = 0.0000001;
            double B_One = (b0 * a3 - b3 * a0) + (b2 * a1 - b1 * a2);
            double C_One = b0 * a1 - b1 * a0;

            Console.WriteLine("{0},{1}    {2},{3}    {4},{5}    {6},{7}", 
                original.GetTopLeftPixel().GetX(),
                original.GetTopLeftPixel().GetY(),
                original.GetTopRightPixel().GetX(),
                original.GetTopRightPixel().GetY(),
                original.GetBottomRightPixel().GetX(),
                original.GetBottomRightPixel().GetY(),
                original.GetBottomLeftPixel().GetX(),
                original.GetBottomLeftPixel().GetY());
            Console.WriteLine("{0},{1}    {2},{3}    {4},{5}    {6},{7}",
                warped.GetTopLeftPixel().GetX(),
                warped.GetTopLeftPixel().GetY(),
                warped.GetTopRightPixel().GetX(),
                warped.GetTopRightPixel().GetY(),
                warped.GetBottomRightPixel().GetX(),
                warped.GetBottomRightPixel().GetY(),
                warped.GetBottomLeftPixel().GetX(),
                warped.GetBottomLeftPixel().GetY());

            List<Pixel> warpedPixels = warped.GetPixels();
            foreach (Pixel pixel in warpedPixels)
            {
                double B = B_One + (b3 * pixel.GetX() - a3 * pixel.GetY());
                double C = C_One + (b1 * pixel.GetX() - a1 * pixel.GetY());

                int originalY = Convert.ToInt32(Math.Min(h - 1, Math.Max(0, (-B + Math.Sqrt(Math.Pow(B, 2) - 4 * A * C)) / (2 * A))));
                int originalX = Convert.ToInt32(Math.Min(w - 1, Math.Max(0, (pixel.GetX() - a0 - a2 * originalY) / (a1 + a3 * originalY))));

                mapping.Add(new Point(pixel.GetX(), pixel.GetY()), new Point(originalX, originalY));

                Console.WriteLine("{0},{1} -> {2},{3}", pixel.GetX(), pixel.GetY(), originalX, originalY);
                //break;
            }
        }
    }
}
