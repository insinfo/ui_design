using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace UIDesign
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();                  
        }
               

        bool isResize = false;
        private const int DRAWING_RECTANGLE = 2;
        private const int DRAWING_START = 1;
        private const int DRAWING_STOP = 2;
        private Element elementSelected;
        private Point mouseDownPosition;
        private Point mouseUpPosition;
        private int toolSelected = 0;
       

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (isResize)
            {
                viewPort.UpdateResize();
            }
            isResize = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewPort.Initialize();
            viewPort.OnSelectElement += ViewPort_OnSelectElement;
            viewPort.OnKeyPress += ViewPort_OnKeyPress;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            viewPort.Release();
        }

        #region Toolbox button click events
        //selection tool
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            toolSelected = 0;
            Mouse.OverrideCursor = Cursors.Arrow;
            viewPort.TransformationToolsEnable = true;
        }
        //add rectangle tool
        private void btnAddRectangle_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Cross;
            toolSelected = DRAWING_RECTANGLE;
            viewPort.TransformationToolsEnable = false;
        }

        private void btnAddEllipse_Click(object sender, RoutedEventArgs e)
        {
            viewPort.AddElement(new Rectangle());
        }

        #endregion

        #region ViewPort events

        private void ViewPort_OnSelectElement(Element el)
        {
            tbElementPositionX.Text = el.X.ToString();
            tbElementPositionY.Text = el.Y.ToString();
            tbElementWidth.Text = el.Width.ToString();
            tbElementHight.Text = el.Height.ToString();
            tbElementRotation.Text = Math.Round(Util.RadianToDegree(el.Rotation), 1).ToString();
        }

        private void ViewPort_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownPosition = e.GetPosition(viewPort);
            switch (toolSelected)
            {
                case DRAWING_RECTANGLE:
                    elementSelected = new Rectangle();
                    elementSelected.X = mouseDownPosition.X;
                    elementSelected.Y = mouseDownPosition.Y;
                    elementSelected.Width = 0;
                    elementSelected.Height = 0;
                    viewPort.AddElement(elementSelected);
                    break;
            }
        }

        private void ViewPort_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseCorrentPosition = e.GetPosition(viewPort);
            switch (toolSelected)
            {
                case DRAWING_RECTANGLE:

                    if (elementSelected != null)
                    {
                        elementSelected.Width = mouseCorrentPosition.X - mouseDownPosition.X;
                        elementSelected.Height = mouseCorrentPosition.Y - mouseDownPosition.Y;
                        viewPort.UpdateDraw();
                    }

                    break;
            }
        }

        private void ViewPort_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseUpPosition = e.GetPosition(viewPort);

            switch (toolSelected)
            {
                case DRAWING_RECTANGLE:

                    if (elementSelected.Width < 2)
                    {
                        viewPort.RemoveElement(elementSelected);
                    }
                    elementSelected = null;

                    break;
            }
        }

        private void ViewPort_OnKeyPress(Key key)
        {
            if (key == Key.Delete)
            {
                if (viewPort.ElementSelected != null)
                {
                    viewPort.RemoveElement(viewPort.ElementSelected);
                }
            }
        }

        #endregion viewPort events

        #region  Property bar KeyDown events
        private void tbElementRotation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (viewPort.ElementSelected != null)
                {
                    double num;
                    if (double.TryParse(tbElementRotation.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out num))
                    {
                        viewPort.ElementSelected.Rotation = Util.DegreeToRadian(num);
                        viewPort.UpdateDraw();
                    }

                }
            }
        }

        private void tbElementWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (viewPort.ElementSelected != null)
                {
                    double num;
                    if (double.TryParse(tbElementWidth.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out num))
                    {
                        viewPort.ElementSelected.Width = num;
                        viewPort.UpdateDraw();
                    }

                }
            }
        }

        private void tbElementHight_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (viewPort.ElementSelected != null)
                {
                    double num;
                    if (double.TryParse(tbElementHight.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out num))
                    {
                        viewPort.ElementSelected.Height = num;
                        viewPort.UpdateDraw();
                    }

                }
            }
        }

        private void tbElementPositionY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (viewPort.ElementSelected != null)
                {
                    double num;
                    if (double.TryParse(tbElementPositionY.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out num))
                    {
                        viewPort.ElementSelected.Y = num;
                        viewPort.UpdateDraw();
                    }

                }
            }
        }

        private void tbElementPositionX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (viewPort.ElementSelected != null)
                {
                    double num;
                    if (double.TryParse(tbElementPositionX.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out num))
                    {
                        viewPort.ElementSelected.X = num;
                        viewPort.UpdateDraw();
                    }

                }
            }
        }



        #endregion

       
    }
}
