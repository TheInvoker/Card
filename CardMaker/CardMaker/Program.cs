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

            Bitmap origImage = new Bitmap("test.png");
            Bitmap warpedImage = new Bitmap("warp3.png");

            //Transformer transformer = new OneFiveOrderPoly();
            //Transformer transformer = new Bilinear();
            Transformer transformer = new Perspective();


            //ColorFilter filter = new Darken();
            ColorFilter filter = new VividLight();

        
            List<KeyValuePair<Shape, Shape>> ShapeList = CreateShapeMapping(origImage, warpedImage, null, null);
            Dictionary<Point, Point> mapping = GenerateWarpedImage(ShapeList, transformer, origImage.Width, origImage.Height);

            origImage.Dispose();
            warpedImage.Dispose();
            //System.IO.File.WriteAllText("mapping.txt", MyDictionaryToJson(mapping));

            Bitmap warpedimage;
            warpedimage = Exporter.GenerateWarpedLogo("tests/colourcircle.png", mapping);
            warpedimage.Save("BITCH1.png");
            warpedimage = Exporter.GenerateWarpedLogo("tests/1414677960_colorful_abstract_design__hd_wallpaper_in_1080p.jpg", mapping);
            warpedimage.Save("BITCH2.png");
            warpedimage = Exporter.GenerateWarpedLogo("tests/4148404-love-abstract-design.jpg", mapping);
            warpedimage.Save("BITCH3.png");
            warpedimage = Exporter.GenerateWarpedLogo("tests/1024px-Burger_King_Logo.svg.png", mapping);
            warpedimage.Save("BITCH4.png");
            warpedimage = Exporter.GenerateWarpedLogo("tests/logotest.png", mapping);
            warpedimage.Save("BITCH5.png");

            Exporter.StampLogo("template/businesscard.png", "FINAL.png", 2780, 1588, 500, 500, warpedimage, filter);
            Exporter.StampLogo("template/black-clip-purse.png", "FINAL2.png", 600, 600, 400, 400, warpedimage, filter);

            
            warpedimage.Dispose();

            Console.WriteLine("Complete!");
            Console.ReadLine();
        }

        private static List<KeyValuePair<Shape, Shape>> CreateShapeMapping(Bitmap origImage, Bitmap warpedImage, string origOut, string warpOut)
        {
            List<Shape> OriginalShapes = SquareDetector.FindSquare(origImage, origOut);
            List<Shape> WarpedSquares = SquareDetector.FindSquare(warpedImage, warpOut);

            List<KeyValuePair<Shape, Shape>> ShapeList = new List<KeyValuePair<Shape, Shape>>();
            foreach (Shape shape in OriginalShapes)
            {
                Shape warpedShape = WarpedSquares.First(x => x.GetTopLeftPixel().GetColor().Equals(shape.GetTopLeftPixel().GetColor()));
                ShapeList.Add(new KeyValuePair<Shape, Shape>(shape, warpedShape));
            }

            Console.WriteLine(string.Format("Found {0} shape pairs", ShapeList.Count));
            return ShapeList;
        }

        private static Dictionary<Point, Point> GenerateWarpedImage(List<KeyValuePair<Shape, Shape>> ShapeList, Transformer transformer, int w, int h)
        {
            Dictionary<Point, Point> mapping = new Dictionary<Point, Point>();

            foreach (KeyValuePair<Shape, Shape> pair in ShapeList)
            {
                transformer.DrawShape(w, h, pair.Key, pair.Value, mapping);
            }

            return mapping;
        }

        private static string MyDictionaryToJson(Dictionary<Point, Point> dict)
        {
            var entries = dict.Select(d => string.Format("\"{0},{1}\":[{2},{3}]", d.Key.X, d.Key.Y, d.Value.X, d.Value.Y));
            return "{" + string.Join(",", entries) + "}";
        }
    }
}
