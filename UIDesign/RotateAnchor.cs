
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;


namespace UIDesign
{/*
    class RotateAnchor : Element
    {
        private RawRectangleF rect;    
        private System.Drawing.Bitmap bmp;
        private System.Drawing.Imaging.BitmapData bmpData;
        private SharpDX.DataStream stream;
        private BitmapProperties bmpProps;
        private SharpDX.Direct2D1.Bitmap rotateIcon;
        private int stride;
        private SharpDX.Size2 bmpSize;      

        public RotateAnchor(RenderTarget renderTarget,double x, double y, string Name, int Index, double Width = 16, double Height = 16)
        {
            this.Direct2D1RenderTarget = renderTarget;
            this.Index = Index;           
            this.X = x ;
            this.Y = y ;
            this.Name = Name;
            this.Width = Width;
            this.Height = Height;

            bmp = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(@"c:\rotateIcon.png");
            bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            stride = bmpData.Stride;

            stream = new SharpDX.DataStream(bmpData.Scan0, stride * bmpData.Height, true, false);
            SharpDX.Direct2D1.PixelFormat pFormat = new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
            bmpProps = new SharpDX.Direct2D1.BitmapProperties(pFormat);

            bmpSize = new SharpDX.Size2(bmp.Width, bmp.Height);

            rotateIcon = new SharpDX.Direct2D1.Bitmap(this.Direct2D1RenderTarget, bmpSize, stream, stride, bmpProps);

            bmp.UnlockBits(bmpData);
            stream.Dispose();
            bmp.Dispose();

            rect = new RawRectangleF();
            rect.Left = (float)X;
            rect.Top = (float)Y;
            rect.Right = (float)(X + Width);
            rect.Bottom = (float)(Y + Height);
        }
        

        public override void Draw(RenderTarget renderTarget)
        {            
        }
        
        public void Draw(RenderTarget renderTarget,double x, double y, double rotation, SharpDX.Vector2 centerPoint)
        {
            this.Direct2D1RenderTarget = renderTarget;
            this.X = x;
            this.Y = y;
            this.Rotation = rotation;
           
            rect.Left = (float)X;
            rect.Top = (float)Y;
            rect.Right = (float)(X + Width);
            rect.Bottom = (float)(Y + Height);

            //Direct2D1RenderTarget.Transform = SharpDX.Matrix3x2.Rotation((float)(this.Rotation), centerPoint);
            this.Direct2D1RenderTarget.DrawBitmap(rotateIcon, rect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            //Direct2D1RenderTarget.Transform = SharpDX.Matrix3x2.Identity;

            //Util.SafeDispose(ref rotateIcon);  
        }

        public override SharpDX.Vector2 GetCenterPoint()
        {
            return new SharpDX.Vector2((float)(this.X + (this.Width / 2)), (float)(this.Y + (this.Height / 2)));
        }

        public override object GetRect()
        {
            return this.rect;
        }

        public override SharpDX.Vector2 GetPosition()
        {
            return new SharpDX.Vector2((float)this.X, (float)this.Y);
        }

        public override bool HitTest(System.Windows.Point mousePosition)
        {
            bool result = false;

            double x = this.X;
            double y = this.Y;
            double width = this.Width;
            double height = this.Height;
            double mousePositionX = mousePosition.X;
            double mousePositionY = mousePosition.Y;
            if ((x <= mousePositionX) && (x + width >= mousePositionX) && (y <= mousePositionY) && (y + height >= mousePositionY))
            {
                result = true;
            }

            return result;
        }

       
    }*/
}
