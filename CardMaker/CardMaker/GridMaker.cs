using System;
using System.Drawing;

namespace CardMaker
{
    class GridMaker
    {

        public static void CreateGrid(int w, int h, int s, string name)
        {
            Bitmap flag = CreateImage(1000, 1000, 100);
            flag.Save(name);
            flag.Dispose();
        }

        private static Bitmap CreateImage(int w, int h, int s)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int q = 40;
            Bitmap flag = new Bitmap(w, h);

            for (int x = 0; x < w; x += s)
            {
                for (int y = 0; y < h; y += s)
                {
                    for (int u = x; u < x + s; u += 1)
                    {
                        for (int v = y; v < y + s; v += 1)
                        {
                            flag.SetPixel(u, v, Color.FromArgb(r, g, b));
                        }
                    }

                    b += q;
                    if (b >= 256)
                    {
                        g += q;
                        b = 0;
                        if (g >= 256)
                        {
                            r += q;
                            g = 0;
                            if (r >= 256)
                            {
                                Console.WriteLine("Color went overboard!");
                                flag.Dispose();
                                return null;
                            }
                        }
                    }
                }
            }

            return flag;
        }
    }
}
