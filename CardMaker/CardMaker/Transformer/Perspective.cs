using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardMaker
{
    class Perspective : Transformer
    {
        public override void DrawShape(Bitmap logo, Bitmap flag, Shape original, Shape warped, Dictionary<string, Point> mapping)
        {
            double[,] X = new double[8, 8];
            double[] Y = new double[8];

            X[0, 0] = 1;
            X[1, 0] = warped.GetTopLeftPixel().GetX();
            X[2, 0] = warped.GetTopLeftPixel().GetY();
            X[3, 0] = 0;
            X[4, 0] = 0;
            X[5, 0] = 0;
            X[6, 0] = warped.GetTopLeftPixel().GetX() * original.GetTopLeftPixel().GetX();
            X[7, 0] = warped.GetTopLeftPixel().GetY() * original.GetTopLeftPixel().GetX();

            X[0, 1] = 1;
            X[1, 1] = warped.GetTopRightPixel().GetX();
            X[2, 1] = warped.GetTopRightPixel().GetY();
            X[3, 1] = 0;
            X[4, 1] = 0;
            X[5, 1] = 0;
            X[6, 1] = warped.GetTopRightPixel().GetX() * original.GetTopRightPixel().GetX();
            X[7, 1] = warped.GetTopRightPixel().GetY() * original.GetTopRightPixel().GetX();

            X[0, 2] = 1;
            X[1, 2] = warped.GetBottomRightPixel().GetX();
            X[2, 2] = warped.GetBottomRightPixel().GetY();
            X[3, 2] = 0;
            X[4, 2] = 0;
            X[5, 2] = 0;
            X[6, 2] = warped.GetBottomRightPixel().GetX() * original.GetBottomRightPixel().GetX();
            X[7, 2] = warped.GetBottomRightPixel().GetY() * original.GetBottomRightPixel().GetX();

            X[0, 3] = 1;
            X[1, 3] = warped.GetBottomLeftPixel().GetX();
            X[2, 3] = warped.GetBottomLeftPixel().GetY();
            X[3, 3] = 0;
            X[4, 3] = 0;
            X[5, 3] = 0;
            X[6, 3] = warped.GetBottomLeftPixel().GetX() * original.GetBottomLeftPixel().GetX();
            X[7, 3] = warped.GetBottomLeftPixel().GetY() * original.GetBottomLeftPixel().GetX();

            X[0, 4] = 0;
            X[1, 4] = 0;
            X[2, 4] = 0;
            X[3, 4] = 1;
            X[4, 4] = warped.GetTopLeftPixel().GetX();
            X[5, 4] = warped.GetTopLeftPixel().GetY();
            X[6, 4] = warped.GetTopLeftPixel().GetX() * original.GetTopLeftPixel().GetY();
            X[7, 4] = warped.GetTopLeftPixel().GetY() * original.GetTopLeftPixel().GetY();

            X[0, 5] = 0;
            X[1, 5] = 0;
            X[2, 5] = 0;
            X[3, 5] = 1;
            X[4, 5] = warped.GetTopRightPixel().GetX();
            X[5, 5] = warped.GetTopRightPixel().GetY();
            X[6, 5] = warped.GetTopRightPixel().GetX() * original.GetTopRightPixel().GetY();
            X[7, 5] = warped.GetTopRightPixel().GetY() * original.GetTopRightPixel().GetY();

            X[0, 6] = 0;
            X[1, 6] = 0;
            X[2, 6] = 0;
            X[3, 6] = 1;
            X[4, 6] = warped.GetBottomRightPixel().GetX();
            X[5, 6] = warped.GetBottomRightPixel().GetY();
            X[6, 6] = warped.GetBottomRightPixel().GetX() * original.GetBottomRightPixel().GetY();
            X[7, 6] = warped.GetBottomRightPixel().GetY() * original.GetBottomRightPixel().GetY();

            X[0, 7] = 0;
            X[1, 7] = 0;
            X[2, 7] = 0;
            X[3, 7] = 1;
            X[4, 7] = warped.GetBottomLeftPixel().GetX();
            X[5, 7] = warped.GetBottomLeftPixel().GetY();
            X[6, 7] = warped.GetBottomLeftPixel().GetX() * original.GetBottomLeftPixel().GetY();
            X[7, 7] = warped.GetBottomLeftPixel().GetY() * original.GetBottomLeftPixel().GetY();

            Y[0] = original.GetTopLeftPixel().GetX();
            Y[1] = original.GetTopRightPixel().GetX();
            Y[2] = original.GetBottomRightPixel().GetX();
            Y[3] = original.GetBottomLeftPixel().GetX();
            Y[4] = original.GetTopLeftPixel().GetY();
            Y[5] = original.GetTopRightPixel().GetY();
            Y[6] = original.GetBottomRightPixel().GetY();
            Y[7] = original.GetBottomLeftPixel().GetY();

            Solver.Solve(X, Y);
            double a0 = Y[0], a1 = Y[1], a2 = Y[2], b0 = Y[3], b1 = Y[4], b2 = Y[5], c1 = Y[6], c2 = Y[7];

            List<Pixel> warpedPixels = warped.GetPixels();
            foreach (Pixel pixel in warpedPixels)
            {
                int originalX = Math.Min(logo.Width - 1, Math.Max(0, Convert.ToInt32((a0 + a1 * pixel.GetX() + a2 * pixel.GetY()) / (1 + c1 * pixel.GetX() + c2 * pixel.GetY()))));
                int originalY = Math.Min(logo.Height - 1, Math.Max(0, Convert.ToInt32((b0 + b1 * pixel.GetX() + b2 * pixel.GetY()) / (1 + c1 * pixel.GetX() + c2 * pixel.GetY()))));

                flag.SetPixel(pixel.GetX(), pixel.GetY(), logo.GetPixel(originalX, originalY));
                mapping.Add(string.Format("{0},{1}", pixel.GetX(), pixel.GetY()), new Point(originalX, originalY));
            }
        }
    }
}
