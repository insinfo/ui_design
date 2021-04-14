using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UIDesign
{
    public class DragAnchor : Element
    {  
        private double offSet;

        public DragAnchor(double x, double y, string Name, int Index, double Width = 10, double Height = 10)
        {
            this.Index = Index;
            offSet = Width/2;
            this.X = x - offSet;
            this.Y = y - offSet;
            this.Name = Name;
            this.Width = Width;
            this.Height = Height;            
        }

        public void Update(double x, double y, double rotation, Point centerPoint)
        {           
            this.X = x - offSet;
            this.Y = y - offSet;
            this.Rotation = rotation;
        }

        public override Polygon DefiningPolygon()
        {
            polygon = new Polygon();
            polygon.Add(new Point(X + Width, Y));
            polygon.Add(new Point(X + Width, Y + Height));
            polygon.Add(new Point(X, Y + Height));

            return polygon;
        }



    }
}
