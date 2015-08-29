namespace CardMaker
{
    class Solver
    {
        public static void Solve(double[,] X_, double[] Y)
        {
            double[,] X = new double[X_.GetLength(0), X_.GetLength(1)];
            for (int l = 0; l < X_.GetLength(1); l++)
                for (int k = 0; k < X_.GetLength(0); k++)
                    X[l, k] = X_[k, l];

            double[,] originalX = (double[,]) X.Clone();

            ComputeCoefficents(X, Y);
        }

        // https://social.msdn.microsoft.com/Forums/en-US/70408584-668d-49a0-b179-fabf101e71e9/solution-of-linear-equations-systems?forum=Vsexpressvcs

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
