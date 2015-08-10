using System;
using System.Collections.Generic;
using System.Drawing;

namespace CardMaker
{
    class OneFiveOrderPoly : Transformer
    {
        public override void DrawShape(int w, int h, Shape original, Shape warped, Dictionary<Point, Point> mapping)
        {
            double[,] X = new double[4, 4];
            double[] Y = new double[4];

            X[0, 0] = 1; X[1, 0] = warped.GetTopLeftPixel().GetX(); X[2, 0] = warped.GetTopLeftPixel().GetY(); X[3, 0] = X[1, 0] * X[2, 0];
            X[0, 1] = 1; X[1, 1] = warped.GetTopRightPixel().GetX(); X[2, 1] = warped.GetTopRightPixel().GetY(); X[3, 1] = X[1, 1] * X[2, 1];
            X[0, 2] = 1; X[1, 2] = warped.GetBottomRightPixel().GetX(); X[2, 2] = warped.GetBottomRightPixel().GetY(); X[3, 2] = X[1, 2] * X[2, 2];
            X[0, 3] = 1; X[1, 3] = warped.GetBottomLeftPixel().GetX(); X[2, 3] = warped.GetBottomLeftPixel().GetY(); X[3, 3] = X[1, 3] * X[2, 3];

            Y[0] = original.GetTopLeftPixel().GetX();
            Y[1] = original.GetTopRightPixel().GetX();
            Y[2] = original.GetBottomRightPixel().GetX();
            Y[3] = original.GetBottomLeftPixel().GetX();

            Solver.Solve(X, Y);
            double a0 = Y[0], a1 = Y[1], a2 = Y[2], a3 = Y[3];

            Y[0] = original.GetTopLeftPixel().GetY();
            Y[1] = original.GetTopRightPixel().GetY();
            Y[2] = original.GetBottomRightPixel().GetY();
            Y[3] = original.GetBottomLeftPixel().GetY();

            Solver.Solve(X, Y);
            double b0 = Y[0], b1 = Y[1], b2 = Y[2], b3 = Y[3];

            List<Pixel> warpedPixels = warped.GetPixels();
            foreach (Pixel pixel in warpedPixels)
            {
                int originalX = Math.Min(w - 1, Math.Max(0, Convert.ToInt32(a0 + a1 * pixel.GetX() + a2 * pixel.GetY() + a3 * pixel.GetX() * pixel.GetY())));
                int originalY = Math.Min(h - 1, Math.Max(0, Convert.ToInt32(b0 + b1 * pixel.GetX() + b2 * pixel.GetY() + b3 * pixel.GetX() * pixel.GetY())));

                mapping.Add(new Point(pixel.GetX(), pixel.GetY()), new Point(originalX, originalY));
            }
        }
    }
}
