using System;
using System.Collections.Generic;
using System.Drawing;

namespace CardMaker
{
    class Perspective : Transformer
    {
        public override void DrawShape(int width, int height, Shape original, Shape warped, Dictionary<Point, Point> mapping)
        {
            double[,] X = new double[8, 8];
            double[] Y = new double[8];

            X[0, 0] = warped.GetTopLeftPixel().GetX();
            X[1, 0] = warped.GetTopLeftPixel().GetY();
            X[2, 0] = 1;
            X[3, 0] = 0;
            X[4, 0] = 0;
            X[5, 0] = 0;
            X[6, 0] = -warped.GetTopLeftPixel().GetX() * original.GetTopLeftPixel().GetX();
            X[7, 0] = -warped.GetTopLeftPixel().GetY() * original.GetTopLeftPixel().GetX();

            X[0, 1] = warped.GetTopRightPixel().GetX();
            X[1, 1] = warped.GetTopRightPixel().GetY();
            X[2, 1] = 1;
            X[3, 1] = 0;
            X[4, 1] = 0;
            X[5, 1] = 0;
            X[6, 1] = -warped.GetTopRightPixel().GetX() * original.GetTopRightPixel().GetX();
            X[7, 1] = -warped.GetTopRightPixel().GetY() * original.GetTopRightPixel().GetX();

            X[0, 2] = warped.GetBottomRightPixel().GetX();
            X[1, 2] = warped.GetBottomRightPixel().GetY();
            X[2, 2] = 1;
            X[3, 2] = 0;
            X[4, 2] = 0;
            X[5, 2] = 0;
            X[6, 2] = -warped.GetBottomRightPixel().GetX() * original.GetBottomRightPixel().GetX();
            X[7, 2] = -warped.GetBottomRightPixel().GetY() * original.GetBottomRightPixel().GetX();

            X[0, 3] = warped.GetBottomLeftPixel().GetX();
            X[1, 3] = warped.GetBottomLeftPixel().GetY();
            X[2, 3] = 1;
            X[3, 3] = 0;
            X[4, 3] = 0;
            X[5, 3] = 0;
            X[6, 3] = -warped.GetBottomLeftPixel().GetX() * original.GetBottomLeftPixel().GetX();
            X[7, 3] = -warped.GetBottomLeftPixel().GetY() * original.GetBottomLeftPixel().GetX();

            X[0, 4] = 0;
            X[1, 4] = 0;
            X[2, 4] = 0;
            X[3, 4] = warped.GetTopLeftPixel().GetX();
            X[4, 4] = warped.GetTopLeftPixel().GetY();
            X[5, 4] = 1;
            X[6, 4] = -warped.GetTopLeftPixel().GetX() * original.GetTopLeftPixel().GetY();
            X[7, 4] = -warped.GetTopLeftPixel().GetY() * original.GetTopLeftPixel().GetY();

            X[0, 5] = 0;
            X[1, 5] = 0;
            X[2, 5] = 0;
            X[3, 5] = warped.GetTopRightPixel().GetX();
            X[4, 5] = warped.GetTopRightPixel().GetY();
            X[5, 5] = 1;
            X[6, 5] = -warped.GetTopRightPixel().GetX() * original.GetTopRightPixel().GetY();
            X[7, 5] = -warped.GetTopRightPixel().GetY() * original.GetTopRightPixel().GetY();

            X[0, 6] = 0;
            X[1, 6] = 0;
            X[2, 6] = 0;
            X[3, 6] = warped.GetBottomRightPixel().GetX();
            X[4, 6] = warped.GetBottomRightPixel().GetY();
            X[5, 6] = 1;
            X[6, 6] = -warped.GetBottomRightPixel().GetX() * original.GetBottomRightPixel().GetY();
            X[7, 6] = -warped.GetBottomRightPixel().GetY() * original.GetBottomRightPixel().GetY();

            X[0, 7] = 0;
            X[1, 7] = 0;
            X[2, 7] = 0;
            X[3, 7] = warped.GetBottomLeftPixel().GetX();
            X[4, 7] = warped.GetBottomLeftPixel().GetY();
            X[5, 7] = 1;
            X[6, 7] = -warped.GetBottomLeftPixel().GetX() * original.GetBottomLeftPixel().GetY();
            X[7, 7] = -warped.GetBottomLeftPixel().GetY() * original.GetBottomLeftPixel().GetY();

            Y[0] = original.GetTopLeftPixel().GetX();
            Y[1] = original.GetTopRightPixel().GetX();
            Y[2] = original.GetBottomRightPixel().GetX();
            Y[3] = original.GetBottomLeftPixel().GetX();
            Y[4] = original.GetTopLeftPixel().GetY();
            Y[5] = original.GetTopRightPixel().GetY();
            Y[6] = original.GetBottomRightPixel().GetY();
            Y[7] = original.GetBottomLeftPixel().GetY();

            Solver.Solve(X, Y);
            double a = Y[0], b = Y[1], c = Y[2], d = Y[3], e = Y[4], f = Y[5], g = Y[6], h = Y[7];

            List<Pixel> warpedPixels = warped.GetPixels();
            foreach (Pixel pixel in warpedPixels)
            {
                double xx = (a * pixel.GetX() + b * pixel.GetY() + c) / (g * pixel.GetX() + h * pixel.GetY() + 1);
                double yy = (d * pixel.GetX() + e * pixel.GetY() + f) / (g * pixel.GetX() + h * pixel.GetY() + 1);
                int originalX = Math.Min(width - 1, Math.Max(0, Convert.ToInt32(xx)));
                int originalY = Math.Min(height - 1, Math.Max(0, Convert.ToInt32(yy)));

                mapping.Add(new Point(pixel.GetX(), pixel.GetY()), new Point(originalX, originalY));
            }
        }
    }
}
