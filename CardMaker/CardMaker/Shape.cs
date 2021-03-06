﻿using System.Collections.Generic;
using System.Linq;

namespace CardMaker
{
    class Shape
    {
        List<Pixel> pixels;
        Pixel[] corners;
        Pixel topleftpixel, toprightpixel, bottomleftpixel, bottomrightpixel;
        int centerx, centery;

        public Shape(List<Pixel> pixels, Pixel[] corners)
        {
            this.pixels = pixels;
            this.corners = corners;

            centerx = corners.Sum(p => p.GetX()) / corners.Length;
            centery = corners.Sum(p => p.GetY()) / corners.Length;

            topleftpixel = corners[0];
            toprightpixel = corners[1];
            bottomrightpixel = corners[2];
            bottomleftpixel = corners[3];
        }

        public void CalculateCorners()
        {
            topleftpixel = corners.First(p => p.GetX() < centerx && p.GetY() < centery);
            toprightpixel = corners.First(p => p.GetX() >= centerx && p.GetY() < centery);
            bottomleftpixel = corners.First(p => p.GetX() < centerx && p.GetY() >= centery);
            bottomrightpixel = corners.First(p => p.GetX() >= centerx && p.GetY() >= centery);
        }

        public List<Pixel> GetPixels()
        {
            return pixels;
        }

        public Pixel GetTopLeftPixel()
        {
            return topleftpixel;
        }

        public Pixel GetTopRightPixel()
        {
            return toprightpixel;
        }

        public Pixel GetBottomLeftPixel()
        {
            return bottomleftpixel;
        }

        public Pixel GetBottomRightPixel()
        {
            return bottomrightpixel;
        }

        public int GetCenterX()
        {
            return centerx;
        }

        public int GetCenterY()
        {
            return centery;
        }
    }
}
