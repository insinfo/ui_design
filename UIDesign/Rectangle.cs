using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;


namespace UIDesign
{
    public class Rectangle : Element
    {
       
        public Rectangle(double X = 50, double Y = 50, double Z = 0, double Width = 100, double Height = 100, Fill Background = null, Fill BorderColor = null, double BorderSize = 2, string Id = "",string Name = "rectangle",bool Visibility = true)
        {           
            this.Id = Id;
            this.Name = Name;
            this.Width = Width;
            this.Height = Height;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Visibility = Visibility;
            this.BorderSize = BorderSize;
            if (Background == null)
            {
                this.Background = new SolidColorFill(new RawColor(150, 150, 150, 200));
            }
            else {
                this.Background = Background;
            }
            if (BorderColor == null)
            {
                this.BorderColor = new SolidColorFill(new RawColor(80, 80, 80, 200));
            }
            else {
                this.BorderColor = BorderColor;
            } 
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
