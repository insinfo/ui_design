
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;


using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace UIDesign
{
    public abstract class D2dControl : System.Windows.Controls.Image
    {
        // - field -----------------------------------------------------------------------

        private SharpDX.Direct3D11.Device device;
        private Texture2D texture2D;
        private Dx11ImageSource dx11ImageSource;
        private RenderTarget d2DRenderTarget;
        private SharpDX.Direct2D1.Factory d2DFactory;
        private Texture2DDescription texture2DDescription;
        private Surface surface;

        private readonly Stopwatch renderTimer = new Stopwatch();
             
        // - property --------------------------------------------------------------------
        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                var isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                return isDesignMode;
            }
        }

        // - public methods --------------------------------------------------------------
        public D2dControl()
        {
            base.Loaded += Window_Loaded;
            base.Unloaded += Window_Closing;
            base.Stretch = System.Windows.Media.Stretch.Fill;

        }

        public abstract void Render(RenderTarget target);
        // - event handler ---------------------------------------------------------------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (D2dControl.IsInDesignMode)
            {
                return;
            }

            texture2DDescription = new Texture2DDescription();
            texture2DDescription.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            texture2DDescription.Format = Format.B8G8R8A8_UNorm;          
            texture2DDescription.MipLevels = 1;
            texture2DDescription.SampleDescription = new SampleDescription(1, 0);
            texture2DDescription.Usage = ResourceUsage.Default;
            texture2DDescription.OptionFlags = ResourceOptionFlags.Shared;
            texture2DDescription.CpuAccessFlags = CpuAccessFlags.None;
            texture2DDescription.ArraySize = 1;

            StartD3D();
            StartRendering();
        }

        private void Window_Closing(object sender, RoutedEventArgs e)
        {
            if (D2dControl.IsInDesignMode)
            {
                return;
            }

            StopRendering();
            EndD3D();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!renderTimer.IsRunning)
            {
                return;
            }
            PrepareAndCallRender();
            dx11ImageSource.InvalidateD3DImage();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            CreateAndBindTargets();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (dx11ImageSource.IsFrontBufferAvailable)
            {
                StartRendering();
            }
            else
            {
                StopRendering();
            }
        }

       /* private static void OnRenderWaitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (D2dControl)d;
            control.dx11ImageSource.RenderWait = (int)e.NewValue;

            MessageBox.Show("sdfs");
        }*/

        // - private methods -------------------------------------------------------------

        private void StartD3D()
        {
            device = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);

            dx11ImageSource = new Dx11ImageSource();
            dx11ImageSource.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            CreateAndBindTargets();

            base.Source = dx11ImageSource;
        }

        private void EndD3D()
        {
            dx11ImageSource.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            base.Source = null;

            Util.SafeDispose(ref d2DRenderTarget);
            Util.SafeDispose(ref d2DFactory);
            Util.SafeDispose(ref dx11ImageSource);
            Util.SafeDispose(ref texture2D);
            Util.SafeDispose(ref device);
        }

        private void CreateAndBindTargets()
        {
            /*SharpDX.Configuration.EnableObjectTracking = true;
            SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects();*/

            dx11ImageSource.SetRenderTarget(null);

            Util.SafeDispose(ref d2DRenderTarget);
            Util.SafeDispose(ref d2DFactory);
            Util.SafeDispose(ref texture2D);
            

            var width = Math.Max((int)ActualWidth, 100);
            var height = Math.Max((int)ActualHeight, 100);
          
            texture2DDescription.Width = width;
            texture2DDescription.Height = height; 

            texture2D = new Texture2D(device, texture2DDescription);
            surface = texture2D.QueryInterface<Surface>();
            
            d2DFactory = new SharpDX.Direct2D1.Factory();
            var rtp = new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied));
            d2DRenderTarget = new RenderTarget(d2DFactory, surface, rtp);

          
            dx11ImageSource.SetRenderTarget(texture2D);
                       

            device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);

            
        }

        private void StartRendering()
        {
            if (renderTimer.IsRunning)
            {
                return;
            }
            System.Windows.Media.CompositionTarget.Rendering += OnRendering;
            renderTimer.Start();
        }

        private void StopRendering()
        {
            if (!renderTimer.IsRunning)
            {
                return;
            }
            System.Windows.Media.CompositionTarget.Rendering -= OnRendering;
            renderTimer.Stop();
        }

        private void PrepareAndCallRender()
        {
            if (device == null)
            {
                return;
            }
            d2DRenderTarget.BeginDraw();
            Render(d2DRenderTarget);
            d2DRenderTarget.EndDraw();
            device.ImmediateContext.Flush();
        }
    }
}
