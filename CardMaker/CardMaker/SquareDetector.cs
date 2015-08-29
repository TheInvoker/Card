using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace CardMaker
{
    class SquareDetector
    {
        public static Boolean[] allPixels;

        public static List<Shape> FindSquare(Bitmap logoImage, string outName)
        {
            List<Shape> ListSquares = new List<Shape>();

            int w = logoImage.Width;
            int h = logoImage.Height;
            int size = w * h;
            allPixels = new Boolean[size];
            for (int i=0; i<size; i+=1)
            {
                Point point = GetPoint(w, i);
                allPixels[i] = logoImage.GetPixel(point.X, point.Y).A > 0;
            }

            int start = 0;
            int squareCount = 0;
            while (allPixels.Contains(true))
            {
                for (int i = start; i < size; i += 1)
                {
                    if (allPixels[i]) {
                        start = i + 1;
                        squareCount += 1;

                        Point randomPoint = GetPoint(w, i);

                        Console.WriteLine(string.Format("Random pixel was {0},{1}", randomPoint.X, randomPoint.Y));

                        List<Pixel> square = BFS(logoImage, randomPoint.X, randomPoint.Y, squareCount);
                        Pixel[] corners = GetCorners(logoImage, square);

                        Shape shape = new Shape(square, corners);
                        shape.CalculateCorners();
                        ListSquares.Add(shape);

                        Console.WriteLine(string.Format("Square size was {0}", square.Count));

                        if (square.Count == 1)
                        {
                            Console.WriteLine("Found bad shape!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        }

                        break;
                    }
                }
            }

            if (outName != null)
            {
                DrawLines(logoImage, ListSquares);
                logoImage.Save(outName);
            }

            Console.WriteLine(string.Format("Complete! Found {0} shapes", ListSquares.Count));

            return ListSquares;
        }

        public static List<Shape> GetShapeFromPoints(List<List<int>> points, string hexcolor, int width, int height, string outName)
        {
            List<Pixel> square = new List<Pixel>();
            ColorConverter cc = new ColorConverter();
            Color color = (Color) cc.ConvertFromString(hexcolor);

            Point p0 = new Point(points.ElementAt(0).ElementAt(0), points.ElementAt(0).ElementAt(1));
            Point p1 = new Point(points.ElementAt(1).ElementAt(0), points.ElementAt(1).ElementAt(1));
            Point p2 = new Point(points.ElementAt(2).ElementAt(0), points.ElementAt(2).ElementAt(1));
            Point p3 = new Point(points.ElementAt(3).ElementAt(0), points.ElementAt(3).ElementAt(1));

            Pixel[] corners = new Pixel[4];
            corners[0] = new Pixel(color, p0.X, p0.Y);
            corners[1] = new Pixel(color, p1.X, p1.Y);
            corners[2] = new Pixel(color, p2.X, p2.Y);
            corners[3] = new Pixel(color, p3.X, p3.Y);

            for (int x=0; x< width; x+=1)
            {
                for(int y=0;y< height;y+=1)
                {
                    if (PointChecker.PointInQuad(new Point(x, y), p0, p1, p2, p3))
                    {
                        square.Add(new Pixel(color, x, y));
                    }
                }
            }

            Shape shape = new Shape(square, corners);
            return new List<Shape>() { shape };
        }

        private static Pixel[] GetCorners(Bitmap image, List<Pixel> square)
        {
            // top left, top right, bottom left, bottom right
            Pixel[] corners = new Pixel[4] { null, null, null, null }; 

            int offset = 5;
            List<KeyValuePair<Pixel, double>> keys = new List<KeyValuePair<Pixel, double>>();
            foreach (Pixel pixel in square)
            {
                List<Pixel> adj = GetNeighBours(image, pixel.GetX(), pixel.GetY());
                if (adj.Count == 4 &&
                    pixel.GetColor().Equals(adj.ElementAt(0).GetColor()) &&
                    pixel.GetColor().Equals(adj.ElementAt(1).GetColor()) &&
                    pixel.GetColor().Equals(adj.ElementAt(2).GetColor()) &&
                    pixel.GetColor().Equals(adj.ElementAt(3).GetColor()))
                {
                    continue;
                }

                double rank = 0;

                for (int x = Math.Max(0, pixel.GetX() - offset); x <= Math.Min(image.Width-1, pixel.GetX() + offset); x += 1)
                {
                    for (int y = Math.Max(0, pixel.GetY() - offset); y <= Math.Min(image.Height-1, pixel.GetY() + offset); y += 1)
                    {
                        if (image.GetPixel(x, y).Equals(pixel.GetColor()))
                        {
                            rank += 1;
                        }
                    }
                }

                keys.Add(new KeyValuePair<Pixel, double>(pixel, rank));
            }

            int i = 1;
            Pixel p;
            int minDistance = 6;
            IEnumerable<KeyValuePair<Pixel, double>> query = keys.OrderBy(x => x.Value);
            
            if (query.Count() == 1)
            {
                Console.WriteLine("Found isolated pixel at {0},{1}", query.ElementAt(0).Key.GetX(), query.ElementAt(0).Key.GetY());
                Console.ReadLine();
                Environment.Exit(1);
            }

            corners[0] = query.ElementAt(0).Key;

            p = query.ElementAt(i).Key;
            while (corners.Any(q => q != null && GetDistance(q, p) < minDistance))
            {
                i += 1;
                p = query.ElementAt(i).Key;
            }
            corners[1] = p;

            p = query.ElementAt(i).Key;
            while (corners.Any(q => q != null && GetDistance(q, p) < minDistance))
            {
                i += 1;
                p = query.ElementAt(i).Key;
            }
            corners[2] = p;

            p = query.ElementAt(i).Key;
            while (corners.Any(q => q != null && GetDistance(q, p) < minDistance))
            {
                i += 1;
                p = query.ElementAt(i).Key;
            }
            corners[3] = p;

            return corners;
        }

        private static void DrawLines(Bitmap logoImage, List<Shape> ListSquares)
        {
            Pen blackPen = new Pen(Color.Red, 1);
            using (var graphics = Graphics.FromImage(logoImage))
            {
                foreach (Shape shape in ListSquares)
                {
                    graphics.DrawLine(blackPen, shape.GetTopLeftPixel().GetX(), shape.GetTopLeftPixel().GetY(), shape.GetTopRightPixel().GetX(), shape.GetTopRightPixel().GetY());
                    graphics.DrawLine(blackPen, shape.GetTopRightPixel().GetX(), shape.GetTopRightPixel().GetY(), shape.GetBottomRightPixel().GetX(), shape.GetBottomRightPixel().GetY());
                    graphics.DrawLine(blackPen, shape.GetBottomRightPixel().GetX(), shape.GetBottomRightPixel().GetY(), shape.GetBottomLeftPixel().GetX(), shape.GetBottomLeftPixel().GetY());
                    graphics.DrawLine(blackPen, shape.GetBottomLeftPixel().GetX(), shape.GetBottomLeftPixel().GetY(), shape.GetTopLeftPixel().GetX(), shape.GetTopLeftPixel().GetY());

                    graphics.DrawLine(blackPen, shape.GetTopLeftPixel().GetX(), shape.GetTopLeftPixel().GetY(), shape.GetBottomRightPixel().GetX(), shape.GetBottomRightPixel().GetY());
                    graphics.DrawLine(blackPen, shape.GetTopRightPixel().GetX(), shape.GetTopRightPixel().GetY(), shape.GetBottomLeftPixel().GetX(), shape.GetBottomLeftPixel().GetY());
                }
            }
        }

        private static List<Pixel> BFS(Bitmap image, int x, int y, int squareCount)
        {
            int w = image.Width;
            int h = image.Height;

            List<Pixel> squares = new List<Pixel>();
            List<Pixel> Q = new List<Pixel>();

            Q.Add(new Pixel(image.GetPixel(x, y), x, y));

            while (Q.Count > 0)
            {
                Pixel u = Q.ElementAt(0);
                Q.RemoveAt(0);
                squares.Add(u);
                allPixels[GetIndex(w, u.GetX(), u.GetY())] = false;

                List<Pixel> adjacentPoints = new List<Pixel>();
                ProcessNeighbour(image, adjacentPoints, u.GetX(), u.GetY() - 1, w, h, u);
                ProcessNeighbour(image, adjacentPoints, u.GetX() + 1, u.GetY(), w, h, u);
                ProcessNeighbour(image, adjacentPoints, u.GetX(), u.GetY() + 1, w, h, u);
                ProcessNeighbour(image, adjacentPoints, u.GetX() - 1, u.GetY(), w, h, u);

                foreach (Pixel point in adjacentPoints)
                {
                    if (!Q.Exists(p => p.GetX() == point.GetX() && p.GetY() == point.GetY()))
                    {
                        Q.Add(point);
                    }
                }
            }

            return squares;
        }

        private static List<Pixel> GetNeighBours(Bitmap image, int x, int y)
        {
            int newx, newy;
            List<Pixel> adj = new List<Pixel>();

            newx = x; newy = y - 1;
            if (InSquare(image.Width, image.Height, newx, newy))
            {
                adj.Add(new Pixel(image.GetPixel(newx, newy), newx, newy));
            }
            newx = x + 1; newy = y;
            if (InSquare(image.Width, image.Height, newx, newy))
            {
                adj.Add(new Pixel(image.GetPixel(newx, newy), newx, newy));
            }
            newx = x; newy = y + 1;
            if (InSquare(image.Width, image.Height, newx, newy))
            {
                adj.Add(new Pixel(image.GetPixel(newx, newy), newx, newy));
            }
            newx = x - 1; newy = y;
            if (InSquare(image.Width, image.Height, newx, newy))
            {
                adj.Add(new Pixel(image.GetPixel(newx, newy), newx, newy));
            }
            return adj;
        }

        private static void ProcessNeighbour(Bitmap image, List<Pixel> adjacentPoints, int newx, int newy, int w, int h, Pixel u)
        {
            int i = GetIndex(w, newx, newy);
            if (InSquare(w, h, newx, newy) && allPixels[i])
            {
                Pixel neighbourPixel = new Pixel(image.GetPixel(newx, newy), newx, newy);
                if (u.GetColor().Equals(neighbourPixel.GetColor()))
                {
                    adjacentPoints.Add(neighbourPixel);
                }
            }
        }

        private static int GetIndex(int w, int x, int y)
        {
            return y * w + x;
        }

        private static double GetDistance(Pixel p1, Pixel p2)
        {
            return Math.Sqrt(Math.Pow(p2.GetX()-p1.GetX(),2) + Math.Pow(p2.GetY() - p1.GetY(), 2));
        }

        private static Point GetPoint(int w, int i)
        {
            int x = i % w;
            return new Point(x, (i - x) / w);
        }

        private static Boolean InSquare(int w, int h, int x, int y)
        {
            return x >= 0 && x < w && y >= 0 && y < h;
        }
    }
}
