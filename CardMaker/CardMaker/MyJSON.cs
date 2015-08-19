using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CardMaker
{
    class MyJSON
    {
        public static void SaveMapping(int width, int height, Dictionary<Point, Point> dict, string transformer, string outPath, string metadataPath)
        {
            StringBuilder builder = new StringBuilder();
            string val = string.Format("{0}", cantor_pair_calculate(width, height));
            builder.Append(val);

            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    Point newPoint = new Point(x, y);
                    if (dict.ContainsKey(newPoint))
                    {
                        Point mappedPoint = dict[newPoint];
                        builder.Append(string.Format(",{0}", cantor_pair_calculate(mappedPoint.X, mappedPoint.Y)));
                    } else
                    {
                        builder.Append(",");
                    }
                }
            }

            string finalstring = builder.ToString();

            Regex rx = new Regex("(,+)($|,\\d)");
            finalstring = rx.Replace(finalstring, new MatchEvaluator(delegate (Match m)
            {
                string commas = m.Groups[1].ToString();
                string next = m.Groups[2].ToString();
                return string.Format("x{0}{1}", commas.Length, next);
            }));

            File.WriteAllText(outPath, finalstring);

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
