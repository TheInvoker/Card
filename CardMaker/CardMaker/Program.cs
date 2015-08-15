using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Newtonsoft.Json;
using System.IO;

namespace CardMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TemplateRef> files = JsonConvert.DeserializeObject<List<TemplateRef>>(File.ReadAllText("master.js"));

            foreach(TemplateRef template in files)
            {
                if (template.active)
                {
                    BatchGenerateMapping(template.grid, template.warp, template.transformer, template.mapping, template.metadata);
                }
            }

            Console.WriteLine("Enter path to image:");
            String logoPath = Console.ReadLine();
            while (!File.Exists(logoPath))
            {
                Console.WriteLine("Invalid path!");
                logoPath = Console.ReadLine();
            }

            foreach (TemplateRef template in files)
            {
                if (template.active)
                {
                    BatchGenerateResult(logoPath, template.template, template.mapping, template.result, template.filter, template.mask, template.x, template.y, template.w, template.h);
                }
            }

            Console.WriteLine("Complete!");
            Console.ReadLine();
        }

        private static Boolean CanSkip(string gridPath, string warpPath, string transformer, string mappingPath, string metadataPath)
        {
            if (File.Exists(mappingPath) && File.Exists(metadataPath))
            {
                FileInfo fi1 = new FileInfo(mappingPath);
                DateTime mappingLastWriteTime = fi1.LastWriteTime;

                FileInfo fi2 = new FileInfo(gridPath);
                DateTime gridLastWriteTime = fi2.LastWriteTime;

                FileInfo fi3 = new FileInfo(warpPath);
                DateTime warpLastWriteTime = fi3.LastWriteTime;

                Dictionary<string, string> metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(metadataPath));

                if (gridLastWriteTime >= mappingLastWriteTime || warpLastWriteTime >= mappingLastWriteTime || !transformer.Equals(metadata["transform"]))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private static void BatchGenerateMapping(string gridPath, string warpPath, string transformer, string mappingPath, string metadataPath)
        {
            if (!CanSkip(gridPath, warpPath, transformer, mappingPath, metadataPath))
            {
                Bitmap origImage = new Bitmap(gridPath);
                Bitmap warpedImage = new Bitmap(warpPath);

                Transformer transformerobj;
                switch (transformer)
                {
                    case "1.5orderpoly":
                        transformerobj = new OneFiveOrderPoly();
                        break;
                    case "bilinear":
                        transformerobj = new Bilinear();
                        break;
                    case "perspective":
                        transformerobj = new Perspective();
                        break;
                    default:
                        throw new InvalidDataException("Invalid transformer argument");
                }

                List<KeyValuePair<Shape, Shape>> ShapeList = CreateShapeMapping(origImage, warpedImage, null, null);

                Dictionary<Point, Point> mapping = GenerateWarpedImage(ShapeList, transformerobj, origImage.Width, origImage.Height);
                MyJSON.SaveMapping(origImage.Width, origImage.Height, mapping, transformer, mappingPath, metadataPath);

                origImage.Dispose();
                warpedImage.Dispose();
            }
        }

        private static void BatchGenerateResult(string logoPath, string templatePath, string mappingPath, string resultPath, string filter, string maskPath, int x, int y, int w, int h)
        {
            ColorFilter filterobj;
            switch (filter)
            {
                case "none":
                    filterobj = new NoFilter();
                    break;
                case "darken":
                    filterobj = new Darken();
                    break;
                case "vividlight":
                    filterobj = new VividLight();
                    break;
                default:
                    throw new InvalidDataException("Invalid filter argument");
            }

            Dictionary<Point, Point> mapping = MyJSON.ReadMapping(mappingPath);

            Bitmap warpedimage = Exporter.GenerateWarpedLogo(logoPath, maskPath, mapping);
            Exporter.StampLogo(templatePath, resultPath, x, y, w, h, warpedimage, filterobj);

            warpedimage.Save("warpedlogo.png");

            warpedimage.Dispose();
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
    }
}
