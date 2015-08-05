using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardMaker
{
    class Solver
    {
        public static void Solve(double[,] X, double[] Y)
        {
            X = new double[4, 4];
            Y = new double[4];

            X[0, 0] = 1;
            X[1, 0] = 1;
            X[2, 0] = 1;
            X[3, 0] = 1;

            X[0, 1] = 52;
            X[1, 1] = 228;
            X[2, 1] = 255;
            X[3, 1] = 0;

            X[0, 2] = 0;
            X[1, 2] = 46;
            X[2, 2] = 229;
            X[3, 2] = 246;

            X[0, 3] = 52*0;
            X[1, 3] = 228*46;
            X[2, 3] = 255*229;
            X[3, 3] = 0 * 246;

            Y[0] = 0;
            Y[1] = 256;
            Y[2] = 0;
            Y[3] = 256;

            double[,] originalX = (double[,]) X.Clone();

            ComputeCoefficents(X, Y);

            double s1 = 0, s2 = 0, s3 = 0, s4 = 0;
            for (int i = 0; i < 4; i += 1)
            {
                s1 += Y[i] * originalX[0, i];
                s2 += Y[i] * originalX[1, i];
                s3 += Y[i] * originalX[2, i];
                s4 += Y[i] * originalX[3, i];
            }

            Console.WriteLine(string.Format("{0} {1} {2} {3}", s1, s2, s3, s4));

            Console.ReadLine();
        }

        public static void ComputeCoefficents(double[,] X, double[] Y)
        {
            int I, J, K, K1, N;
            N = Y.Length;
            for (K = 0; K < N; K++)
            {
                K1 = K + 1;
                for (I = K; I < N; I++)
                {
                    if (X[I, K] != 0)
                    {
                        for (J = K1; J < N; J++)
                        {
                            X[I, J] /= X[I, K];
                        }
                        Y[I] /= X[I, K];
                    }
                }
                for (I = K1; I < N; I++)
                {
                    if (X[I, K] != 0)
                    {
                        for (J = K1; J < N; J++)
                        {
                            X[I, J] -= X[K, J];
                        }
                        Y[I] -= Y[K];
                    }
                }
            }
            for (I = N - 2; I >= 0; I--)
            {
                for (J = N - 1; J >= I + 1; J--)
                {
                    Y[I] -= X[I, J] * Y[J];
                }
            }
        }
    }
}
