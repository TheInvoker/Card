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
            string root = "../../../../../PlutoMakeJava/PlutoMakeJava/bin/";
            List<TemplateRefList> files = JsonConvert.DeserializeObject<List<TemplateRefList>>(File.ReadAllText(root + "master.js"));

            foreach (TemplateRefList templatelist in files)
            {
                foreach (TemplateRef template in templatelist.angles)
                {
                    if (template.active)
                    {
                        BatchGenerateMapping(
                            root + template.grid,
                            root + template.warp,
                            template.transformer,
                            root + template.mapping,
                            root + template.metadata
                        );
                    }
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

                MetaDataRef metadata = JsonConvert.DeserializeObject<MetaDataRef>(File.ReadAllText(metadataPath));

                if (gridLastWriteTime >= mappingLastWriteTime || warpLastWriteTime >= mappingLastWriteTime || !transformer.Equals(metadata.transform))
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
            if (true || !CanSkip(gridPath, warpPath, transformer, mappingPath, metadataPath))
            {
                int width, height;
                List<Shape> gridShapes, warpShapes;
                QuadRef quadref;

                if (gridPath.StartsWith("["))
                {
                    quadref = JsonConvert.DeserializeObject<QuadRef>(File.ReadAllText(gridPath));
                    List<List<int>> points = quadref.points;
                    List<int> p1 = points.ElementAt(0);
                    List<int> p2 = points.ElementAt(1);
                    List<int> p3 = points.ElementAt(2);
                    width = p2.ElementAt(0) - p1.ElementAt(0) + 1;
                    height = p3.ElementAt(1) - p2.ElementAt(1) + 1;

                    gridShapes = SquareDetector.GetShapeFromPoints(points, quadref.hexcolor, width, height, null);
                } else
                {
                    Bitmap origImage = new Bitmap(gridPath);
                    width = origImage.Width;
                    height = origImage.Height;

                    gridShapes = SquareDetector.FindSquare(origImage, null);
                    origImage.Dispose();
                }
                if (warpPath.StartsWith("["))
                {
                    quadref = JsonConvert.DeserializeObject<QuadRef>(File.ReadAllText(gridPath));

                    warpShapes = SquareDetector.GetShapeFromPoints(quadref.points, quadref.hexcolor, width, height, null);
                } else
                {
                    Bitmap warpedImage = new Bitmap(warpPath);

                    warpShapes = SquareDetector.FindSquare(warpedImage, null);
                    warpedImage.Dispose();
                }

                
                List<KeyValuePair<Shape, Shape>> ShapeList = FindPairing(gridShapes, warpShapes);

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

                Dictionary<Point, Point> mapping = GenerateMapping(ShapeList, transformerobj, width, height);
                MyJSON.SaveMapping(width, height, mapping, transformer, mappingPath, metadataPath);
            }
        }




        private static List<KeyValuePair<Shape, Shape>> FindPairing(List<Shape> OriginalShapes, List<Shape> WarpedSquares)
        {
            List<KeyValuePair<Shape, Shape>> ShapeList = new List<KeyValuePair<Shape, Shape>>();
            foreach (Shape shape in OriginalShapes)
            {
                Shape warpedShape = WarpedSquares.First(x => x.GetTopLeftPixel().GetColor().Equals(shape.GetTopLeftPixel().GetColor()));
                ShapeList.Add(new KeyValuePair<Shape, Shape>(shape, warpedShape));
            }

            Console.WriteLine(string.Format("Found {0} shape pairs", ShapeList.Count));
            return ShapeList;
        }

        private static Dictionary<Point, Point> GenerateMapping(List<KeyValuePair<Shape, Shape>> ShapeList, Transformer transformer, int w, int h)
        {
            Dictionary<Point, Point> mapping = new Dictionary<Point, Point>();

            foreach (KeyValuePair<Shape, Shape> pair in ShapeList)
            {
                transformer.GenerateMapping(w, h, pair.Key, pair.Value, mapping);
            }

            return mapping;
        }
    }
}
