/*using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;

using System.Drawing;

using SlimDX;
using SlimDX.Direct3D9;


           //Obtém o nível de depuração solicitado
           DebugLevel debugLevel = DebugLevel.None;

           //Recursos para a renderização do Direct2D
           Factory d2dFactory = new Factory(FactoryType.SingleThreaded, debugLevel);

            WindowInteropHelper windowHwnd = new WindowInteropHelper(this);                     
           int largura = Convert.ToInt32(200);
           int altura = Convert.ToInt32(100);                                    

            //Definir as propriedades de renderização
            WindowRenderTargetProperties renderTargetProps = new WindowRenderTargetProperties();
           renderTargetProps.PixelSize = new System.Drawing.Size(largura, altura);
           renderTargetProps.Handle = windowHwnd.Handle;
           renderTargetProps.PresentOptions = PresentOptions.Immediately;

           //Criar o destino de renderização
           WindowRenderTarget renderTargt = new WindowRenderTarget(d2dFactory, renderTargetProps);

           SlimDX.Color4 azulColor = new SlimDX.Color4(System.Drawing.Color.LightSteelBlue);
           SlimDX.Color4 vermelhoColor = new SlimDX.Color4(System.Drawing.Color.Red);
           SlimDX.Direct2D.SolidColorBrush pincelRed = new SlimDX.Direct2D.SolidColorBrush(renderTargt, vermelhoColor);
           System.Drawing.Rectangle retangulo = new System.Drawing.Rectangle(20, 20, largura - 40, altura - 40);

           //faz um desenho de um retagulo
           renderTargt.BeginDraw();
           renderTargt.Clear(azulColor);
           renderTargt.DrawRectangle(pincelRed, retangulo);
           renderTargt.EndDraw();

namespace UIDesign
{
    public class D3DControl : DockPanel
    {
        // we use it for 3D
        Direct3D direct3D;
        Direct3DEx direct3DEx;
        Device device;
        DeviceEx deviceEx;
        PresentParameters pp;

        // this one is our only child
        System.Windows.Controls.Image image;
        D3DImage d3dimage;
        bool StartThread = false;
        bool sizeChanged = false;

        // some public properties
        public bool useDeviceEx
        {
            get;
            private set;
        }

        public Direct3D Direct3D
        {
            get
            {
                if (useDeviceEx)
                    return direct3DEx;
                else
                    return direct3D;
            }
        }

        public Device Device
        {
            get
            {
                if (useDeviceEx)
                    return deviceEx;
                else
                    return device;
            }
        }

        #region Events

        /// <summary>
        /// Occurs once per iteration of the main loop.
        /// </summary>
        public event EventHandler MainLoop;

        /// <summary>
        /// Occurs when the device is created.
        /// </summary>
        public event EventHandler DeviceCreated;

        /// <summary>
        /// Occurs when the device is destroyed.
        /// </summary>
        public event EventHandler DeviceDestroyed;

        /// <summary>
        /// Occurs when the device is lost.
        /// </summary>
        public event EventHandler DeviceLost;

        /// <summary>
        /// Occurs when the device is reset.
        /// </summary>
        public event EventHandler DeviceReset;

        /// <summary>
        /// Raises the OnInitialize event.
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:MainLoop"/> event.
        /// </summary>
        protected virtual void OnMainLoop(EventArgs e)
        {
            if (MainLoop != null)
                MainLoop(this, e);
        }

        /// <summary>
        /// Raises the DeviceCreated event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceCreated(EventArgs e)
        {
            if (DeviceCreated != null)
                DeviceCreated(this, e);
        }

        /// <summary>
        /// Raises the DeviceDestroyed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceDestroyed(EventArgs e)
        {
            if (DeviceDestroyed != null)
                DeviceDestroyed(this, e);
        }

        /// <summary>
        /// Raises the DeviceLost event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceLost(EventArgs e)
        {
            if (DeviceLost != null)
                DeviceLost(this, e);
        }

        /// <summary>
        /// Raises the DeviceReset event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceReset(EventArgs e)
        {
            if (DeviceReset != null)
                DeviceReset(this, e);
        }

        #endregion

        public D3DControl()
        {
            image = new System.Windows.Controls.Image();
            d3dimage = new D3DImage();
            image.Source = d3dimage;

            Children.Clear();
            Children.Add(image);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            InitializeDirect3D();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            sizeChanged = true;
        }

        void InitializeDirect3D()
        {
            try
            {
                direct3DEx = new Direct3DEx();
                useDeviceEx = true;
            }
            catch
            {
                direct3D = new Direct3D();
                useDeviceEx = false;
            }
        }

        /// <summary>
        /// Initializes the various Direct3D objects we'll be using.
        /// </summary>
        public bool Initialize(bool startThread)
        {
            try
            {
                StartThread = startThread;

                ReleaseD3D();
                HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "test", IntPtr.Zero);

                pp = new PresentParameters();
                pp.SwapEffect = SwapEffect.Discard;
                pp.DeviceWindowHandle = hwnd.Handle;
                pp.Windowed = true;
                pp.BackBufferWidth = (int)ActualWidth;
                pp.BackBufferHeight = (int)ActualHeight;
                pp.BackBufferFormat = Format.X8R8G8B8;

                if (useDeviceEx)
                {
                    deviceEx = new DeviceEx((Direct3DEx)Direct3D, 0,
                                        DeviceType.Hardware,
                                        hwnd.Handle,
                                        CreateFlags.HardwareVertexProcessing,
                                        pp);
                }
                else
                {
                    device = new Device(Direct3D, 0,
                                        DeviceType.Hardware,
                                        hwnd.Handle,
                                        CreateFlags.HardwareVertexProcessing,
                                        pp);
                }

                // call the users one
                OnDeviceCreated(EventArgs.Empty);
                OnDeviceReset(EventArgs.Empty);

                // only if startThread is true
                if (StartThread)
                {
                    CompositionTarget.Rendering += OnRendering;
                    d3dimage.IsFrontBufferAvailableChanged += new DependencyPropertyChangedEventHandler(OnIsFrontBufferAvailableChanged);
                }
                d3dimage.Lock();
                d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, Device.GetBackBuffer(0, 0).ComPointer);
                d3dimage.Unlock();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ReleaseD3D()
        {
            if (device != null)
            {
                if (!device.Disposed)
                {
                    device.Dispose();
                    device = null;
                }
            }
            d3dimage.Lock();
            d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            d3dimage.Unlock();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            Result result;

            try
            {
                if (Device == null)
                    Initialize(StartThread);

                if (sizeChanged)
                {
                    pp.BackBufferWidth = (int)ActualWidth;
                    pp.BackBufferHeight = (int)ActualHeight;
                    Device.Reset(pp);
                    OnDeviceReset(EventArgs.Empty);
                }

                if (d3dimage.IsFrontBufferAvailable)
                {
                    result = Device.TestCooperativeLevel();
                    if (result.IsFailure)
                    {
                        throw new Direct3D9Exception();
                    }
                    d3dimage.Lock();

                    Device.Clear(ClearFlags.Target, new Color4(System.Drawing.Color.Yellow), 0, 0);
                    Device.BeginScene();
                    // call the users method
                    OnMainLoop(EventArgs.Empty);

                    Device.EndScene();
                    Device.Present();

                    d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, Device.GetBackBuffer(0, 0).ComPointer);
                    d3dimage.AddDirtyRect(new Int32Rect(0, 0, d3dimage.PixelWidth, d3dimage.PixelHeight));
                    d3dimage.Unlock();
                }
            }
            catch (Direct3D9Exception ex)
            {
                string msg = ex.Message;
                Initialize(StartThread);
            }
            sizeChanged = false;
        }

        void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (d3dimage.IsFrontBufferAvailable)
            {
                Initialize(StartThread);
            }
            else
            {
                CompositionTarget.Rendering -= OnRendering;
            }
        }

    }
}
*/