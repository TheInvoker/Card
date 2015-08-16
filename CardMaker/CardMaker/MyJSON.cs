using Newtonsoft.Json;
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

            builder.Append(string.Format("{0},{1}", width, height));
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    Point newPoint = new Point(x, y);
                    if (dict.ContainsKey(newPoint))
                    {
                        Point mappedPoint = dict[newPoint];
                        builder.Append(string.Format(",{0},{1}", mappedPoint.X, mappedPoint.Y));
                    } else
                    {
                        builder.Append(",,");
                    }
                }
            }
            string finalstring = builder.ToString();
            
            Regex rx = new Regex("(,,+)($|,\\d)");
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

        public static Dictionary<Point, Point> ReadMapping(string mappingPath)
        {
            string finalstring = File.ReadAllText(mappingPath);

            Regex rx = new Regex("(x?\\d+)");
            MatchCollection mc = rx.Matches(finalstring);
            Dictionary<Point, Point> data = new Dictionary<Point, Point>();
            Point newPoint = new Point(0,0);

            int state = 0;
            int width = 0;
            int x = 0, y = 0;
            foreach (Match m in mc)
            {
                string val = m.ToString();

                if (state == 0)
                {
                    width = System.Convert.ToInt32(val);
                    state += 1;
                } else if (state == 1)
                {
                    state += 1;
                } else if (val.Contains("x")) {
                    int commalength = System.Convert.ToInt32(val.Substring(1))/2;
                    int curentIndex = width * y + x;
                    int newIndex = curentIndex + commalength;
                    y = newIndex / width;
                    x = newIndex - (width * y);
                } else
                {
                    if (state == 2)
                    {
                        newPoint.X = System.Convert.ToInt32(val);
                        state += 1;
                    } else
                    {
                        newPoint.Y = System.Convert.ToInt32(val);
                        data.Add(new Point(x, y), newPoint);
                        newPoint = new Point(0, 0);
                        state = 2;
                    
                        x += 1;
                        if (x >= width)
                        {
                            x = 0;
                            y += 1;
                        }
                    }
                }
            }

            return data;
        }
    }
}
