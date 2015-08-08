﻿
using System.Collections.Generic;
using System.Drawing;
using System;

namespace CardMaker
{
    class Bilinear : Transformer
    {
        public override void DrawShape(int w, int h, Shape original, Shape warped, Dictionary<Point, Point> mapping)
        {
            double offset = original.GetTopRightPixel().GetX() - original.GetTopLeftPixel().GetX();

            double oTopLeftX = original.GetTopLeftPixel().GetX() / offset;
            double oTopLeftY = original.GetTopLeftPixel().GetY() / offset;
            double oTopRightX = original.GetTopRightPixel().GetX() / offset;
            double oTopRightY = original.GetTopRightPixel().GetY() / offset;
            double oBottomRightX = original.GetBottomRightPixel().GetX() / offset;
            double oBottomRightY = original.GetBottomRightPixel().GetY() / offset;
            double oBottomLeftX = original.GetBottomLeftPixel().GetX() / offset;
            double oBottomLeftY = original.GetBottomLeftPixel().GetY() / offset;
            double wTopLeftX = warped.GetTopLeftPixel().GetX() / offset;
            double wTopLeftY = warped.GetTopLeftPixel().GetY() / offset;
            double wTopRightX = warped.GetTopRightPixel().GetX() / offset;
            double wTopRightY = warped.GetTopRightPixel().GetY() / offset;
            double wBottomRightX = warped.GetBottomRightPixel().GetX() / offset;
            double wBottomRightY = warped.GetBottomRightPixel().GetY() / offset;
            double wBottomLeftX = warped.GetBottomLeftPixel().GetX() / offset;
            double wBottomLeftY = warped.GetBottomLeftPixel().GetY() / offset;

            double[,] X = new double[4, 4];
            double[] Y = new double[4];

            X[0, 0] = 1; X[1, 0] = oTopLeftX; X[2, 0] = oTopLeftY; X[3, 0] = X[1, 0] * X[2, 0];
            X[0, 1] = 1; X[1, 1] = oTopRightX; X[2, 1] = oTopRightY; X[3, 1] = X[1, 1] * X[2, 1];
            X[0, 2] = 1; X[1, 2] = oBottomRightX; X[2, 2] = oBottomRightY; X[3, 2] = X[1, 2] * X[2, 2];
            X[0, 3] = 1; X[1, 3] = oBottomLeftX; X[2, 3] = oBottomLeftY; X[3, 3] = X[1, 3] * X[2, 3];

            Y[0] = wTopLeftX;
            Y[1] = wTopRightX;
            Y[2] = wBottomRightX;
            Y[3] = wBottomLeftX;

            Solver.Solve(X, Y);
            double a0 = Y[0], a1 = Y[1], a2 = Y[2], a3 = Y[3];

            Y[0] = wTopLeftY;
            Y[1] = wTopRightY;
            Y[2] = wBottomRightY;
            Y[3] = wBottomLeftY;

            Solver.Solve(X, Y);
            double b0 = Y[0], b1 = Y[1], b2 = Y[2], b3 = Y[3];



            double A = (b2 * a3) - (b3 * a2);
            double B_One = (b0 * a3 - b3 * a0) + (b2 * a1 - b1 * a2);
            double C_One = b0 * a1 - b1 * a0;

            List<Pixel> warpedPixels = warped.GetPixels();
            foreach (Pixel pixel in warpedPixels)
            {
                double pX = pixel.GetX() / offset;
                double pY = pixel.GetY() / offset;

                double B = B_One + (b3 * pX - a3 * pY);
                double C = C_One + (b1 * pX - a1 * pY);

                double q = -0.5 * (B + Math.Sqrt(Math.Pow(B, 2) - 4 * A * C));
                double originalY_ = C / q;
                double t = a1 + a3 * originalY_;
                double originalX_ = (pX - a0 - a2 * originalY_) / (t);

                int originalX = Math.Min(w - 1, Math.Max(0, Convert.ToInt32(Math.Max(0, Math.Min(w - 1, originalX_ * offset)))));
                int originalY = Math.Min(h - 1, Math.Max(0, Convert.ToInt32(Math.Max(0, Math.Min(h - 1, originalY_ * offset)))));

                mapping.Add(new Point(pixel.GetX(), pixel.GetY()), new Point(originalX, originalY));
            }
        }
    }
}
