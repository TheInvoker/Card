using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace CardMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            //Bitmap flag = CreateImage(1000, 1000, 100);
            //flag.Save("test.png");
            //flag.Dispose();

            //CompareImage("test.png", "warped.png");
            //Dictionary<String, List<Point>> mapping = CreateMapping("test.png", "warped.png");

            //createLogo("logo.png", mapping);
            //System.IO.File.WriteAllText("out.txt", MyDictionaryToJson(mapping));

            //FindDark("shirt.png");


            
            List<Shape> OriginalShapes = SquareDetector.FindSquare("test.png");
            List<Shape> WarpedSquares = SquareDetector.FindSquare("warp2.png");

            List<KeyValuePair<Shape, Shape>> ShapeList = new List<KeyValuePair<Shape, Shape>>();
            foreach (Shape shape in OriginalShapes)
            {
                Shape warpedShape = WarpedSquares.First(x => x.GetTopLeftPixel().GetColor().Equals(shape.GetTopLeftPixel().GetColor()));
                ShapeList.Add(new KeyValuePair<Shape, Shape>(shape, warpedShape));
            }
            Console.WriteLine(string.Format("Found {0} shape pairs", ShapeList.Count));


            //Transformer transformer = new OneFiveOrderPoly();
            Transformer transformer = new Bilinear();

            Bitmap logo = new Bitmap("logotest.png");
            Bitmap flag = new Bitmap(logo.Width, logo.Height);

            foreach (KeyValuePair<Shape, Shape> pair in ShapeList)
            {
                transformer.DrawShape(logo, flag, pair.Key, pair.Value);
            }

            flag.Save("logooutput.png");
            logo.Dispose();
            flag.Dispose();


            Console.WriteLine("Complete!");
            Console.ReadLine();
        }

        private static void FindDark(string logoFilePath)
        {
            Bitmap logoImage = new Bitmap(logoFilePath);
            int w = logoImage.Width;
            int h = logoImage.Height;
            Bitmap flag = new Bitmap(w, h);

            for (int x = 0; x < w; x += 1)
            {
                for (int y = 0; y < h; y += 1)
                {
                    Color pixel = logoImage.GetPixel(x, y);
                    double brightness = 0.2126*pixel.R + 0.7152*pixel.G + 0.0722*pixel.B;

                    if (brightness < 150)
                    {
                        flag.SetPixel(x, y, Color.Red);
                    } else
                    {
                        flag.SetPixel(x, y, pixel);
                    }
                }
            }

            flag.Save("newlogo.png");
            flag.Dispose();
        }

        private static void createLogo(string logoFilePath, Dictionary<String, List<Point>> mapping)
        {
            Bitmap logoImage = new Bitmap(logoFilePath);
            int w = logoImage.Width;
            int h = logoImage.Height;
            Bitmap flag = new Bitmap(w, h);

            for(int x=0; x < w; x+=1)
            {
                for (int y = 0; y < h; y += 1)
                {
                    Color pixel = logoImage.GetPixel(x, y);
                    string key = string.Format("{0},{1}", x, y);
                    List<Point> points = mapping[key];

                    if (points.Count == 0)
                    {
                        flag.SetPixel(x, y, pixel);
                    } else
                    {
                        foreach (Point point in points)
                        {
                            flag.SetPixel(point.X, point.Y, pixel);
                        }
                    }
                }
            }

            flag.Save("newlogo.png");
            flag.Dispose();
        }

        private static Dictionary<String, List<Point>> CreateMapping(string origFilePath, string warpedFilePath)
        {
            Dictionary<String, List<Point>> mapping = new Dictionary<String, List<Point>>();

            Bitmap origImage = new Bitmap(origFilePath);
            Bitmap warpedImage = new Bitmap(warpedFilePath);

            int w = origImage.Width;
            int h = origImage.Height;

            for (int x = 0; x < w; x += 1)
            {
                for (int y = 0; y < h; y += 1)
                {
                    Color pixel = origImage.GetPixel(x, y);
                    List<Point> points = GetPoints(warpedImage, pixel);
                    mapping.Add(string.Format("{0},{1}", x, y), points);
                }
            }

            return mapping;
        }

        private static List<Point> GetPoints(Bitmap image, Color pixel)
        {
            int w = image.Width;
            int h = image.Height;
            List<Point> pointlist = new List<Point>();

            for (int x = 0; x < w; x += 1)
            {
                for (int y = 0; y < h; y += 1)
                {
                    Color pixel2 = image.GetPixel(x, y);
                    if (pixel.Equals(pixel2))
                    {
                        pointlist.Add(new Point(x, y));
                        return pointlist;
                    }
                }
            }

            return pointlist;
        }

        private static string MyDictionaryToJson(Dictionary<string, List<Point>> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": {1}", d.Key, ListPointsToString(d.Value)));
            return "{" + string.Join(",", entries) + "}";
        }

        private static string ListPointsToString(List<Point> points)
        {
            List<string> strlist = new List<string>();
            foreach(Point point in points)
            {
                strlist.Add(string.Format("[{0},{1}]", point.X, point.Y));
            }
            return "[" + string.Join(",", strlist) + "]";
        }

        private static Bitmap CreateImage(int w, int h, int s)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int q = 40;
            Bitmap flag = new Bitmap(w, h);

            for(int x=0; x<w; x+=s)
            {
                for(int y=0; y<h; y+=s)
                {
                    for (int u = x; u < x+s; u += 1)
                    {
                        for (int v = y; v < y+s; v += 1)
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
