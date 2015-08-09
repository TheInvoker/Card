using System;
using System.Collections.Generic;
using System.Drawing;

namespace CardMaker
{
    class Perspective : Transformer
    {
        public override void DrawShape(int width, int height, Shape original, Shape warped, Dictionary<Point, Point> mapping)
        {
            double xOff = original.GetTopLeftPixel().GetX();
            double yOff = original.GetTopLeftPixel().GetY();

            double oTopLeftX = 0;
            double oTopLeftY = 0;
            double oTopRightX = (original.GetTopRightPixel().GetX() - xOff);
            double oTopRightY = (original.GetTopRightPixel().GetY() - yOff);
            double oBottomRightX = (original.GetBottomRightPixel().GetX() - xOff);
            double oBottomRightY = (original.GetBottomRightPixel().GetY() - yOff);
            double oBottomLeftX = (original.GetBottomLeftPixel().GetX() - xOff);
            double oBottomLeftY = (original.GetBottomLeftPixel().GetY() - yOff);
            double wTopLeftX = (warped.GetTopLeftPixel().GetX() - xOff);
            double wTopLeftY = (warped.GetTopLeftPixel().GetY() - yOff);
            double wTopRightX = (warped.GetTopRightPixel().GetX() - xOff);
            double wTopRightY = (warped.GetTopRightPixel().GetY() - yOff);
            double wBottomRightX = (warped.GetBottomRightPixel().GetX() - xOff);
            double wBottomRightY = (warped.GetBottomRightPixel().GetY() - yOff);
            double wBottomLeftX = (warped.GetBottomLeftPixel().GetX() - xOff);
            double wBottomLeftY = (warped.GetBottomLeftPixel().GetY() - yOff);

            double[,] X = new double[8, 8];
            double[] Y = new double[8];

            X[0, 0] = wTopLeftX;
            X[1, 0] = wTopLeftY;
            X[2, 0] = 1;
            X[3, 0] = 0;
            X[4, 0] = 0;
            X[5, 0] = 0;
            X[6, 0] = -wTopLeftX * oTopLeftX;
            X[7, 0] = -wTopLeftY * oTopLeftX;

            X[0, 1] = wTopRightX;
            X[1, 1] = wTopRightY;
            X[2, 1] = 1;
            X[3, 1] = 0;
            X[4, 1] = 0;
            X[5, 1] = 0;
            X[6, 1] = -wTopRightX * oTopRightX;
            X[7, 1] = -wTopRightY * oTopRightX;

            X[0, 2] = wBottomRightX;
            X[1, 2] = wBottomRightY;
            X[2, 2] = 1;
            X[3, 2] = 0;
            X[4, 2] = 0;
            X[5, 2] = 0;
            X[6, 2] = -wBottomRightX * oBottomRightX;
            X[7, 2] = -wBottomRightY * oBottomRightX;

            X[0, 3] = wBottomLeftX;
            X[1, 3] = wBottomLeftY;
            X[2, 3] = 1;
            X[3, 3] = 0;
            X[4, 3] = 0;
            X[5, 3] = 0;
            X[6, 3] = -wBottomLeftX * oBottomLeftX;
            X[7, 3] = -wBottomLeftY * oBottomLeftX;

            X[0, 4] = 0;
            X[1, 4] = 0;
            X[2, 4] = 0;
            X[3, 4] = wTopLeftX;
            X[4, 4] = wTopLeftY;
            X[5, 4] = 1;
            X[6, 4] = -wTopLeftX * oTopLeftY;
            X[7, 4] = -wTopLeftY * oTopLeftY;

            X[0, 5] = 0;
            X[1, 5] = 0;
            X[2, 5] = 0;
            X[3, 5] = wTopRightX;
            X[4, 5] = wTopRightY;
            X[5, 5] = 1;
            X[6, 5] = -wTopRightX * oTopRightY;
            X[7, 5] = -wTopRightY * oTopRightY;

            X[0, 6] = 0;
            X[1, 6] = 0;
            X[2, 6] = 0;
            X[3, 6] = wBottomRightX;
            X[4, 6] = wBottomRightY;
            X[5, 6] = 1;
            X[6, 6] = -wBottomRightX * oBottomRightY;
            X[7, 6] = -wBottomRightY * oBottomRightY;

            X[0, 7] = 0;
            X[1, 7] = 0;
            X[2, 7] = 0;
            X[3, 7] = wBottomLeftX;
            X[4, 7] = wBottomLeftY;
            X[5, 7] = 1;
            X[6, 7] = -wBottomLeftX * oBottomLeftY;
            X[7, 7] = -wBottomLeftY * oBottomLeftY;

            Y[0] = oTopLeftX;
            Y[1] = oTopRightX;
            Y[2] = oBottomRightX;
            Y[3] = oBottomLeftX;
            Y[4] = oTopLeftY;
            Y[5] = oTopRightY;
            Y[6] = oBottomRightY;
            Y[7] = oBottomLeftY;

            Solver.Solve(X, Y);
            double a = Y[0], b = Y[1], c = Y[2], d = Y[3], e = Y[4], f = Y[5], g = Y[6], h = Y[7];

            List<Pixel> warpedPixels = warped.GetPixels();
            foreach (Pixel pixel in warpedPixels)
            {
                double pX = (pixel.GetX() - xOff);
                double pY = (pixel.GetY() - yOff);

                double t1_ = (a * pX + b * pY + c);
                double t2_ = (g * pX + h * pY + 1);
                double t3_ = (d * pX + e * pY + f);

                //if (t2_ == 0)
                //{
                //    t2_ = 0.000000001;
                //}

                double originalX_ = t1_ / t2_;
                double originalY_ = t3_ / t2_;

                int originalX = Math.Min(width - 1, Math.Max(0, Convert.ToInt32(Math.Max(0, Math.Min(width, originalX_ + xOff)))));
                int originalY = Math.Min(height - 1, Math.Max(0, Convert.ToInt32(Math.Max(0, Math.Min(height, originalY_ + yOff)))));

                mapping.Add(new Point(pixel.GetX(), pixel.GetY()), new Point(originalX, originalY));
            }
        }
    }
}
