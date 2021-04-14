using System;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using SharpDX.IO;
using System.Windows;

namespace UIDesign
{
    public class Util
    {
        public static float ColorFloatToByte(Byte byteValue)
        {
            float result = (float)byteValue / 255;
            return result;
        }

        public static RawColor4 ARGBColorToRawColor4(byte a, byte r, byte g, byte b)
        {
            RawColor4 result = new RawColor4(ColorFloatToByte(r), ColorFloatToByte(g), ColorFloatToByte(b), ColorFloatToByte(a));
            return result;
        }

        public static RawColor4 RGBColorToRawColor4(byte r, byte g, byte b)
        {
            RawColor4 result = new RawColor4(ColorFloatToByte(r), ColorFloatToByte(g), ColorFloatToByte(b), ColorFloatToByte(255));
            return result;
        }

        // Grau para Radiano
        public static double DegreeToRadian(double angle)
        {            
            return Math.PI * angle / 180.0;
        }

        // Radiano para Grau
        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        

        public static void SafeDispose<T>(ref T resource) where T : class
        {
            if (resource == null)
            {
                return;
            }

            var disposer = resource as IDisposable;
            if (disposer != null)
            {
                try
                {
                    disposer.Dispose();
                }
                catch
                {
                }
            }

            resource = null;
        }

        //Retorna um SharpDX.Direct2D1.Bitmap de um arquivo
        private SharpDX.Direct2D1.Bitmap LoadBitmapFromFile(RenderTarget Direct2D1RenderTarget, string filename)
        {
            System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(filename);

            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(
                    new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            SharpDX.DataStream stream = new SharpDX.DataStream(bmpData.Scan0, bmpData.Stride * bmpData.Height, true, false);
            SharpDX.Direct2D1.PixelFormat pFormat = new SharpDX.Direct2D1.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
            BitmapProperties bmpProps = new SharpDX.Direct2D1.BitmapProperties(pFormat);

            SharpDX.Direct2D1.Bitmap result = new SharpDX.Direct2D1.Bitmap(Direct2D1RenderTarget, new SharpDX.Size2(bmp.Width, bmp.Height), stream, bmpData.Stride, bmpProps);

            bmp.UnlockBits(bmpData);

            stream.Dispose();
            bmp.Dispose();

            return result;
        }

        private SharpDX.Direct2D1.Bitmap1 LoadBitmapFromContentFile(SharpDX.Direct2D1.DeviceContext d2dContext, string filePath)
        {
            SharpDX.Direct2D1.Bitmap1 newBitmap;

            // Neccessary for creating WIC objects.
            ImagingFactory imagingFactory = new ImagingFactory();
            NativeFileStream fileStream = new NativeFileStream(filePath, NativeFileMode.Open, NativeFileAccess.Read);

            // Used to read the image source file.
            BitmapDecoder bitmapDecoder = new BitmapDecoder(imagingFactory, fileStream, DecodeOptions.CacheOnDemand);

            // Get the first frame of the image.
            BitmapFrameDecode frame = bitmapDecoder.GetFrame(0);

            // Convert it to a compatible pixel format.
            FormatConverter converter = new FormatConverter(imagingFactory);
            converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPRGBA);

            // Create the new Bitmap1 directly from the FormatConverter.
            newBitmap = SharpDX.Direct2D1.Bitmap1.FromWicBitmap(d2dContext, converter);

            Util.SafeDispose(ref bitmapDecoder);
            Util.SafeDispose(ref fileStream);
            Util.SafeDispose(ref imagingFactory);

            return newBitmap;
        }
    }
}
