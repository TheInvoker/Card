using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CardMaker
{
    class MyJSON
    {
        public static void SaveMapping(int width, int height, Dictionary<Point, Point> dict, string transformer, string outPath, string metadataPath)
        {
            BinaryWriter b = new BinaryWriter(File.Open(outPath, FileMode.Create));

            List<int> ints = new List<int>();
            ints.Add(cantor_pair_calculate(width, height));

            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    Point newPoint = new Point(x, y);
                    if (dict.ContainsKey(newPoint))
                    {
                        Point mappedPoint = dict[newPoint];
                        ints.Add(cantor_pair_calculate(mappedPoint.X, mappedPoint.Y));
                    } else
                    {
                        if (ints[ints.Count-1] < 0)
                        {
                            ints[ints.Count-1] -= 1;
                        } else
                        {
                            ints.Add(-1);
                        }
                    }
                }
            }

            foreach(int i in ints)
            {
                b.Write(i);
            }

            b.Dispose();
            b.Close();

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
    }
}
