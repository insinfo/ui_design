using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

//SharpDX
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System.Windows;

namespace UIDesign
{
    class Direct2DRender : RenderBackend 
    {
        private SharpDX.Direct3D11.Device Direct3D11Device;
        private SharpDX.Direct3D11.Texture2D Direct3D11Texture2D;
        private SharpDX.Direct2D1.RenderTarget Direct2D1RenderTarget;
        private SharpDX.Direct2D1.Factory Direct2D1Factory;
        private SharpDX.Direct3D11.Texture2DDescription Direct3D11Texture2DDescription;
        private SharpDX.DXGI.Surface DXGISurface;
        private SharpDX.Direct2D1.PixelFormat Direct2D1PixelFormat;
        private SharpDX.Direct2D1.RenderTargetProperties Direct2D1RenderTargetProperties;

        private SharpDX.Direct3D9.DeviceEx Direct3D9DeviceEx;
        private SharpDX.Direct3D9.Texture Direct3D9Texture;
        private SharpDX.Direct3D9.Surface Direct3D9Surface;
        private SharpDX.Direct3D9.Direct3DEx D3DContext;
        private SharpDX.Direct3D9.PresentParameters presentParams;
        private SharpDX.Direct3D9.CreateFlags createFlags;
        private SharpDX.Direct3D9.Format direct3D9Format;
        private IntPtr sharedHandle;
        private D3DImage d3DImageSource;

        private int textureWidth;
        private int textureHeight;
        private System.Windows.Controls.Image ImageControl;
               

        //Inicializa tudo
        public override void Initialize(System.Windows.Controls.Image imageControl)
        {
            this.ImageControl = imageControl;
            this.textureWidth = (int)imageControl.Width;
            this.textureHeight = (int)imageControl.Height;            

            ElementCollection = new List<Element>();

            RenderEngineInit();
            Draw();
            RenderImageSourceInit();
        }
                
        //Limpa a area de desenho
        public override void Clear()
        {
            Direct2D1RenderTarget.Clear(Util.RGBColorToRawColor4(255, 255, 255));
        }

        //Desenha
        public override void Draw()
        {           
            //Aqui inicia o desenho 
            Direct2D1RenderTarget.BeginDraw();
            Clear();

            foreach (Element element in ElementCollection)
            {
                if (element.Visibility)
                {
                    Point[] polygonSegments = element.DefiningPolygon().GetAllSegment();
                    RawColor backColor = element.Background.SolidColor;

                    SolidColorBrush fillColor = new SolidColorBrush(Direct2D1RenderTarget, Util.ARGBColorToRawColor4(backColor.A, backColor.R, backColor.G, backColor.B));

                    PathGeometry geometry = new PathGeometry(Direct2D1Factory);

                    GeometrySink geometrySink = geometry.Open();

                    geometrySink.BeginFigure(new RawVector2((float)element.X, (float)element.Y), FigureBegin.Filled);

                    foreach (Point segment in polygonSegments)
                    {
                        geometrySink.AddLine(new RawVector2((float)segment.X, (float)segment.Y));
                    }

                    geometrySink.EndFigure(FigureEnd.Closed);
                    geometrySink.Close();

                    this.Direct2D1RenderTarget.FillGeometry(geometry, fillColor);

                    Util.SafeDispose(ref geometrySink);
                    Util.SafeDispose(ref geometry);
                    Util.SafeDispose(ref fillColor);
                }
            }

            //Finaliza o desenho
            Direct2D1RenderTarget.EndDraw();           
            Direct3D11Device.ImmediateContext.Flush();            
        }

        //Redesenha
        public override void UpdateDraw()
        {          
            Draw();
            d3DImageSource.Lock();
            d3DImageSource.AddDirtyRect(new System.Windows.Int32Rect(0, 0, d3DImageSource.PixelWidth, d3DImageSource.PixelHeight));
            d3DImageSource.Unlock();
        }

        //Redesenha e redimensiona a area de desenho
        public override void UpdateDrawOnResize(double actualWidth,double actualHeight)
        {
            try
            {
                //Direct2D Reset
                Util.SafeDispose(ref Direct2D1RenderTarget);
                Util.SafeDispose(ref Direct2D1Factory);
                Util.SafeDispose(ref Direct3D11Texture2D);

                textureWidth = Math.Max((int)actualWidth, 100);
                textureHeight = Math.Max((int)actualHeight, 100);
                Direct3D11Texture2DDescription.Width = textureWidth;
                Direct3D11Texture2DDescription.Height = textureHeight;

                RenderEngineConfig();

                //ImageSource Reset    
                Direct3D9Texture = null;
                d3DImageSource.Lock();
                d3DImageSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                d3DImageSource.Unlock();
                RenderImageSourceConfig();

                UpdateDraw();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        /*---------------------------------- Direct2D -----------------------------------------*/
        internal override void RenderEngineInit()
        {
            //Inicialiso o dispositivo Direct2D
            Direct3D11Device = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);

            //Defino as parametros da Textura 2D do Direct3D 11 
            Direct3D11Texture2DDescription = new Texture2DDescription();
            Direct3D11Texture2DDescription.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            Direct3D11Texture2DDescription.Format = Format.B8G8R8A8_UNorm;
            Direct3D11Texture2DDescription.MipLevels = 1;
            Direct3D11Texture2DDescription.SampleDescription = new SampleDescription(1, 0);
            Direct3D11Texture2DDescription.Usage = ResourceUsage.Default;
            Direct3D11Texture2DDescription.OptionFlags = ResourceOptionFlags.Shared;
            Direct3D11Texture2DDescription.CpuAccessFlags = CpuAccessFlags.None;
            Direct3D11Texture2DDescription.ArraySize = 1;
            Direct3D11Texture2DDescription.Width = textureWidth;
            Direct3D11Texture2DDescription.Height = textureHeight;

            //Inicialiso o PixelFormat do Direct2D
            Direct2D1PixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied);

            //Inicialiso o RenderTargetProperties do Direct2D
            Direct2D1RenderTargetProperties = new RenderTargetProperties(Direct2D1PixelFormat);

            RenderEngineConfig();
            //Direct3D11Device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);

            
        }

        internal override void RenderEngineConfig()
        {
            //Cria uma textura 2D Direct3D 11 
            Direct3D11Texture2D = new Texture2D(Direct3D11Device, Direct3D11Texture2DDescription);

            //Inicialiso a superficie DXGI do Direct3D 11
            DXGISurface = Direct3D11Texture2D.QueryInterface<Surface>();

            //Inicializo o Factory do Direct2D
            Direct2D1Factory = new SharpDX.Direct2D1.Factory();

            //Inicialiso o RenderTarget do Direct2D       
            Direct2D1RenderTarget = new RenderTarget(Direct2D1Factory, DXGISurface, Direct2D1RenderTargetProperties);

            //Direct3D11Device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);
        }

        /*---------------------------------- ImageSorce -----------------------------------------*/
        internal override void RenderImageSourceInit()
        {
            //Cria uma Instancia da class WPF D3DImage
            d3DImageSource = new D3DImage();

            // Cria o objeto D3D, que é necessário para criar o D3DDevice. 
            D3DContext = new SharpDX.Direct3D9.Direct3DEx();

            // Configurar a estrutura usada para criar o D3DDevice. 
            presentParams = new SharpDX.Direct3D9.PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard;
            presentParams.DeviceWindowHandle = NativeMethods.GetDesktopWindow();
            presentParams.PresentationInterval = SharpDX.Direct3D9.PresentInterval.Default;

            //Cria as Flags Direct3D 9
            createFlags = SharpDX.Direct3D9.CreateFlags.HardwareVertexProcessing | SharpDX.Direct3D9.CreateFlags.Multithreaded | SharpDX.Direct3D9.CreateFlags.FpuPreserve;

            // Criar o dispositivo Direct3D 9 
            Direct3D9DeviceEx = new SharpDX.Direct3D9.DeviceEx(D3DContext, 0, SharpDX.Direct3D9.DeviceType.Hardware, IntPtr.Zero, createFlags, presentParams);
            direct3D9Format = SharpDX.Direct3D9.Format.A8R8G8B8;

            RenderImageSourceConfig();

            ImageControl.Source = d3DImageSource;

            d3DImageSource.Lock();
            d3DImageSource.AddDirtyRect(new System.Windows.Int32Rect(0, 0, d3DImageSource.PixelWidth, d3DImageSource.PixelHeight));
            d3DImageSource.Unlock();
        }

        internal override void RenderImageSourceConfig()
        {
            sharedHandle = Direct3D11Texture2D.QueryInterface<SharpDX.DXGI.Resource>().SharedHandle;

            //Criar uma textura Direct3D 9 
            Direct3D9Texture = new SharpDX.Direct3D9.Texture(Direct3D9DeviceEx, Direct3D11Texture2D.Description.Width, Direct3D11Texture2D.Description.Height, 1, SharpDX.Direct3D9.Usage.RenderTarget, direct3D9Format, SharpDX.Direct3D9.Pool.Default, ref sharedHandle);

            //Criar uma superficie Direct3D 9 
            Direct3D9Surface = Direct3D9Texture.GetSurfaceLevel(0);

            d3DImageSource.Lock();
            d3DImageSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, Direct3D9Surface.NativePointer);
            d3DImageSource.Unlock();
        }

        //Libera os recursos da memoria
        public override void Release()
        {
            //Direct2D
            Util.SafeDispose(ref Direct2D1RenderTarget);
            Util.SafeDispose(ref Direct2D1Factory);
            Util.SafeDispose(ref d3DImageSource);
            Util.SafeDispose(ref Direct3D11Texture2D);
            Util.SafeDispose(ref Direct3D11Device);

            //ImageSource
            Util.SafeDispose(ref Direct3D9Texture);
            Util.SafeDispose(ref Direct3D9Surface);
            Util.SafeDispose(ref Direct3D9DeviceEx);
            Util.SafeDispose(ref D3DContext);
        }
    }
}
