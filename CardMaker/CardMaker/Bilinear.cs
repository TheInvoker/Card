using System.Drawing;

namespace CardMaker
{
    class Bilinear : Transformer
    {
        public override void DrawShape(Bitmap logo, Bitmap flag, Shape original, Shape warped)
        {
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

            Solver.Solve(X, Y);
            double a0 = Y[0], a1 = Y[1], a2 = Y[2], a3 = Y[3];

            Y[0] = warped.GetTopLeftPixel().GetY();
            Y[1] = warped.GetTopRightPixel().GetY();
            Y[2] = warped.GetBottomRightPixel().GetY();
            Y[3] = warped.GetBottomLeftPixel().GetY();

            Solver.Solve(X, Y);
            double b0 = Y[0], b1 = Y[1], b2 = Y[2], b3 = Y[3];
        }
    }
}
