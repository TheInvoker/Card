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
            //GridMaker.CreateGrid(1000, 1000, 100, "test.png");
            //Exporter.ExportLogo("logo.png", "newlogo.png", mapping);


            List<KeyValuePair<Shape, Shape>> ShapeList = CreateShapeMapping("test.png", "output.png", "warp2.png", "output.png");

            Transformer transformer = new OneFiveOrderPoly();
            //Transformer transformer = new Bilinear();
            //Transformer transformer = new Perspective();

            Dictionary<string, Point> mapping = GenerateWarpedImage(ShapeList, transformer, "tests/colourcircle.png", "logooutput.png");
            System.IO.File.WriteAllText("mapping.txt", MyDictionaryToJson(mapping));

            Console.WriteLine("Complete!");
            Console.ReadLine();
        }

        private static List<KeyValuePair<Shape, Shape>> CreateShapeMapping(string origName, string origOut, string warpName, string warpOut)
        {
            List<Shape> OriginalShapes = SquareDetector.FindSquare(origName, origOut);
            List<Shape> WarpedSquares = SquareDetector.FindSquare(warpName, warpOut);

            List<KeyValuePair<Shape, Shape>> ShapeList = new List<KeyValuePair<Shape, Shape>>();
            foreach (Shape shape in OriginalShapes)
            {
                Shape warpedShape = WarpedSquares.First(x => x.GetTopLeftPixel().GetColor().Equals(shape.GetTopLeftPixel().GetColor()));
                ShapeList.Add(new KeyValuePair<Shape, Shape>(shape, warpedShape));
            }

            Console.WriteLine(string.Format("Found {0} shape pairs", ShapeList.Count));
            return ShapeList;
        }

        private static Dictionary<string, Point> GenerateWarpedImage(List<KeyValuePair<Shape, Shape>> ShapeList, Transformer transformer, string inName, string outName)
        {
            Bitmap logo = new Bitmap(inName);
            Bitmap flag = new Bitmap(logo.Width, logo.Height);
            Dictionary<string, Point> mapping = new Dictionary<string, Point>();

            foreach (KeyValuePair<Shape, Shape> pair in ShapeList)
            {
                transformer.DrawShape(logo, flag, pair.Key, pair.Value, mapping);
            }

            flag.Save(outName);
            logo.Dispose();
            flag.Dispose();

            return mapping;
        }

        private static string MyDictionaryToJson(Dictionary<string, Point> dict)
        {
            var entries = dict.Select(d => string.Format("\"{0}\": [{1},{2}]", d.Key, d.Value.X, d.Value.Y));
            return "{" + string.Join(",", entries) + "}";
        }
    }
}
