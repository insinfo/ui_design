
using System;
using System.Collections.Generic;
using System.Windows;

namespace UIDesign
{
    public abstract class Element
    {
        internal Polygon polygon;
        public int Index { get; set; } = 0;
        public double Rotation { get; set; } = 0;
        public string Id { get; set; }
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;
        public Fill Background { get; set; }
        public Fill BorderColor { get; set; }
        public double BorderSize { get; set; } = 2;
        public bool Visibility { get; set; } = true;
        public abstract Polygon DefiningPolygon();        
      
        public bool IsPointInPolygon(Point p, Point[] polygon)
        {
            double minX = polygon[0].X;
            double maxX = polygon[0].X;
            double minY = polygon[0].Y;
            double maxY = polygon[0].Y;
            for (int i = 1; i < polygon.Length; i++)
            {
                Point q = polygon[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                     p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public Point GetCenterPoint()
        {
            return new Point((this.X + (this.Width / 2)), (this.Y + (this.Height / 2)));
        }

        public Point GetPosition()
        {
            return new Point(this.X, this.Y);
        }

        public bool HitTest(Point mousePosition)
        {
            return IsPointInPolygon(mousePosition, polygon.GetAllSegment());
        }
    }

    public class Polygon
    {
        private List<Point> segmentCollection;

        public Polygon()
        {
            segmentCollection = new List<Point>();
        }

        public void Add(Point segment)
        {
            segmentCollection.Add(segment);
        }
        public void Remove(Point segment)
        {
            segmentCollection.Remove(segment);
        }
        public Point[] GetAllSegment()
        {
            return this.segmentCollection.ToArray();
        }
    }

    public abstract class Fill
    {
        public RawColor SolidColor { get; set; }

    }

    public class SolidColorFill : Fill
    {
        
        public SolidColorFill(RawColor color)
        {
            this.SolidColor = color;
        }
    }

    public class RawColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }     
        public byte B { get; set; }
      
        public RawColor(byte r, byte g, byte b, byte a)
        {           
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;           
        }

        public float ColorByteToFloat(Byte byteValue)
        {
            float result = (float)byteValue / 255;
            return result;
        }        

        public string ToRGBString()
        {
            return R + ":" + G + ":" + B + ":" + A;
        }

        public string ToRawString()
        {
            return ColorByteToFloat(R) + ":" + ColorByteToFloat(G) + ":" + ColorByteToFloat(B) + ":" + ColorByteToFloat(A);
        }
    }
}
