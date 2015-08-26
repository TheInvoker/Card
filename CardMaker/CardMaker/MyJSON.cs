using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CardMaker
{
    class MyJSON
    {
        public static void SaveMapping(int width, int height, Dictionary<Point, Point> dict, string transformer, string outPath, string metadataPath)
        {
            Bitmap b = new Bitmap(width, height);

            int[] buffer = new int[3];
            int max = 256 * 256 * 256;

            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    Point newPoint = new Point(x, y);
                    if (dict.ContainsKey(newPoint))
                    {
                        Point mappedPoint = dict[newPoint];
                        int c = cantor_pair_calculate(mappedPoint.X, mappedPoint.Y);

                        if (c >= max)
                        {
                            throw new InvalidDataException("Cantor pairing function value too big!");
                        }

                        longToRgb(c, buffer);

                        b.SetPixel(x, y, Color.FromArgb(255, buffer[0], buffer[1], buffer[2]));
                    } else
                    {
                        b.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                    }
                }
            }

            b.Save(outPath);
            b.Dispose();

            string json = "{\"transform\":\"" + transformer + "\", \"width\":" + width.ToString() + ", \"height\":" + height.ToString() + "}";
            File.WriteAllText(metadataPath, json);
        }

        /**
         * Calculate a unique integer based on two integers (cantor pairing).
         */
        private static int cantor_pair_calculate(int x, int y)
        {
            return ((x + y) * (x + y + 1)) / 2 + y;
        }


        private static void longToRgb(long rgb, int[] buffer)
        {
            int r = (int)((rgb & 0xFF0000) >> 16);
            int g = (int)((rgb & 0x00FF00) >> 8);
            int b = (int)(rgb & 0x0000FF);

            buffer[0] = r;
            buffer[1] = g;
            buffer[2] = b;
        }
    }
}
