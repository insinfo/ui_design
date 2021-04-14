
using System;
using System.Collections.Generic;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;


namespace UIDesign
{
    class ViewPort : System.Windows.Controls.Grid
    {
        private RenderBackend  renderBackend;
        private MainWindow currentWindow;
        private UserActivityHook actHook;

        private bool redraw = false;
     
        public Element ElementSelected;
        private System.Windows.Point dragOffset;
        //private List<DragAnchor> dragAnchorCollection;
        //private List<RotateAnchor> rotateAnchorCollection;

        private System.Windows.Controls.Image imageControl;
          
        public bool TransformationToolsEnable = false;
        private bool dragging = false;
        private bool draggingResizer = false;
        private bool draggingRotate = false;

        //private DragAnchor dragAnchorSelected = null;
        //private RotateAnchor rotateAnchorSelected = null;
        private Point mouseDownPosition;
        private Point mouseUpPosition;
        private double clickAngle = 0;

        //Events
        //Cria os Delegate
        public delegate void SelectElement(Element el);
        public delegate void KeyPress(Key key);
        //Cria os eventos
        public event SelectElement OnSelectElement;
        public event KeyPress OnKeyPress;

        public ViewPort()
        {
            
        }

        //Inicializa tudo
        public void Initialize()
        {  
            currentWindow = (MainWindow)Application.Current.MainWindow;
                 
            actHook = new UserActivityHook(); // create an instance
            // hang on events
            actHook.OnMouseActivity += ActHook_OnMouseActivity;

            //se for true desenha na tela
            redraw = true;

            
            imageControl = new System.Windows.Controls.Image();
            imageControl.Stretch = System.Windows.Media.Stretch.None;
            imageControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            imageControl.VerticalAlignment = VerticalAlignment.Stretch;
            imageControl.MouseLeftButtonDown += ImageControl_MouseLeftButtonDown;
            imageControl.MouseUp += ImageControl_MouseUp;
            imageControl.Focusable = true;
            imageControl.KeyDown += D2DCanvas_KeyDown;

            imageControl.Width = (int)this.ActualWidth;
            imageControl.Height = (int)this.ActualHeight;

            this.Children.Add(imageControl);

            renderBackend = new Direct2DRender();
            renderBackend.Initialize(imageControl);
                                    
            /*
            DragAnchor top_left = new DragAnchor(0, 0, "top-left", 0);
            renderBackend.ElementCollection.Add(top_left);
            DragAnchor top_middle = new DragAnchor(0, 0, "top-middle", 1);
            renderBackend.ElementCollection.Add(top_middle);
            DragAnchor top_right = new DragAnchor(0, 0, "top-right", 2);
            renderBackend.ElementCollection.Add(top_right);
            DragAnchor middle_left = new DragAnchor(0, 0, "middle-left", 3);
            renderBackend.ElementCollection.Add(middle_left);
            DragAnchor middle_right = new DragAnchor(0, 0, "middle-right", 4);
            renderBackend.ElementCollection.Add(middle_right);
            DragAnchor bottom_left = new DragAnchor(0, 0, "bottom-left", 5);
            renderBackend.ElementCollection.Add(bottom_left);
            DragAnchor bottom_middle = new DragAnchor(0, 0, "bottom-middle", 6);
            renderBackend.ElementCollection.Add(bottom_middle);
            DragAnchor bottom_right = new DragAnchor(0, 0, "bottom-right", 7);
            renderBackend.ElementCollection.Add(bottom_right);
            /*RotateAnchor rotateAnchor = new RotateAnchor(0, 0, "rotate", 1);          
            rotateAnchorCollection.Add(rotateAnchor);*/
        }

        private void D2DCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyPress(e.Key);
        }

        //Mouse dow event
        private void ImageControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
          /*  imageControl.Focus();
            if (TransformationToolsEnable)
            {
                mouseDownPosition = e.GetPosition(imageControl);
                Element elementSelec = ContainsElement(mouseDownPosition);
                DragAnchor anchorSelec = ContainsAnchor(mouseDownPosition);
                RotateAnchor rotateAnchorSelec = ContainsRotateAnchor(mouseDownPosition);

                if (elementSelec != null)
                {
                    dragOffset.X = (mouseDownPosition.X - elementSelec.X);
                    dragOffset.Y = (mouseDownPosition.Y - elementSelec.Y);
                    ElementSelected = elementSelec;

                    OnSelectElement(ElementSelected);

                    if (anchorSelec == null)
                    {
                        dragging = true;
                    }
                    else
                    {
                        draggingResizer = true;
                        dragAnchorSelected = anchorSelec;
                       
                    }
                }
                else if (anchorSelec != null)
                {
                    if (ElementSelected != null)
                    {
                        dragging = false;
                        draggingResizer = true;
                        dragAnchorSelected = anchorSelec;                        
                    }
                }
                else if (rotateAnchorSelec != null)
                {
                    if (ElementSelected != null)
                    {
                        draggingRotate = true;
                        dragging = false;
                        draggingResizer = false;
                        rotateAnchorSelected = rotateAnchorSelec;

                        var cX = ElementSelected.X + ElementSelected.Width * 0.1;
                        var cY = ElementSelected.Y + ElementSelected.Height * 0.1;
                        // calculate click angle minus the last angle
                        clickAngle = getAngle(cX, cY, mouseDownPosition.X, mouseDownPosition.Y) - ElementSelected.Rotation;
                    }
                }
                else
                {
                    draggingRotate = false;
                    ElementSelected = null;
                    dragAnchorSelected = null;
                    dragging = false;
                    draggingResizer = false;
                }

                UpdateDraw();
            }*/
        }

        //Mouse move event
        private void ActHook_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           /* if (TransformationToolsEnable)
            {
                Point locationFromScreen;
                System.Windows.Point correntMousePosition;

                if (imageControl != null)
                {
                    locationFromScreen = imageControl.PointToScreen(new Point(0, 0));
                    correntMousePosition = new System.Windows.Point(0, 0);

                    if (dragging)
                    {
                        correntMousePosition.X = (e.X - locationFromScreen.X - dragOffset.X);
                        correntMousePosition.Y = (e.Y - locationFromScreen.Y - dragOffset.Y);

                        ElementSelected.X = correntMousePosition.X;
                        ElementSelected.Y = correntMousePosition.Y;

                        UpdateDraw();
                        OnSelectElement(ElementSelected);
                    }
                    if (draggingResizer)
                    {
                        correntMousePosition.X = (e.X - locationFromScreen.X);
                        correntMousePosition.Y = (e.Y - locationFromScreen.Y);

                        var oldx = ElementSelected.X;
                        var oldy = ElementSelected.Y;

                        switch (dragAnchorSelected.Index)
                        {
                            case 0:
                                ElementSelected.X = correntMousePosition.X;
                                ElementSelected.Y = correntMousePosition.Y;
                                ElementSelected.Width += oldx - correntMousePosition.X;
                                ElementSelected.Height += oldy - correntMousePosition.Y;
                                break;
                            case 1:
                                ElementSelected.Y = correntMousePosition.Y;
                                ElementSelected.Height += oldy - correntMousePosition.Y;
                                break;
                            case 2:
                                ElementSelected.Y = correntMousePosition.Y;
                                ElementSelected.Width = correntMousePosition.X - oldx;
                                ElementSelected.Height += oldy - correntMousePosition.Y;
                                break;
                            case 3:
                                ElementSelected.X = correntMousePosition.X;
                                ElementSelected.Width += oldx - correntMousePosition.X;
                                break;
                            case 4:
                                ElementSelected.Width = correntMousePosition.X - oldx;
                                break;
                            case 5:
                                ElementSelected.X = correntMousePosition.X;
                                ElementSelected.Width += oldx - correntMousePosition.X;
                                ElementSelected.Height = correntMousePosition.Y - oldy;
                                break;
                            case 6:
                                ElementSelected.Height = correntMousePosition.Y - oldy;
                                break;
                            case 7:
                                ElementSelected.Width = correntMousePosition.X - oldx;
                                ElementSelected.Height = correntMousePosition.Y - oldy;
                                break;
                        }
                        UpdateDraw();
                        OnSelectElement(ElementSelected);
                    }
                    if (draggingRotate)
                    {
                        correntMousePosition.X = (e.X - locationFromScreen.X);
                        correntMousePosition.Y = (e.Y - locationFromScreen.Y);

                        //Opção de rotação 2
                        var cX = ElementSelected.X + ElementSelected.Width * 0.1;
                        var cY = ElementSelected.Y + ElementSelected.Height * 0.1;

                        if (ElementSelected != null)
                        {
                            ElementSelected.Rotation = (getAngle(cX, cY, correntMousePosition.X, correntMousePosition.Y) - clickAngle);
                            UpdateDraw();
                            OnSelectElement(ElementSelected);
                        }

                    }
                }
            }*/

        }
        
        //Mouse up event
        private void ImageControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
           /* if (TransformationToolsEnable)
            {
                mouseUpPosition = e.GetPosition(imageControl);
                dragging = false;
                draggingResizer = false;
                draggingRotate = false;
               
            }*/
        }

        // angle helper function       
        private double getAngle(double cX, double cY, double mX, double mY)
        {
            var angle = Math.Atan2(mY - cY, mX - cX);
            return angle;
        }

        private Element ContainsElement(Point mousePosition)
        {
            Element result = null;
            foreach (Element ele in renderBackend.ElementCollection)
            {
                if (ele.HitTest(mousePosition))
                {
                    result = ele;
                }
            }
            return result;
        }
        /*private DragAnchor ContainsAnchor(Point mousePosition)
        {
            DragAnchor result = null;
            foreach (DragAnchor ele in dragAnchorCollection)
            {
                if (ele.HitTest(mousePosition))
                {
                    result = ele;
                }
            }
            return result;
        }
        private RotateAnchor ContainsRotateAnchor(Point mousePosition)
        {
            RotateAnchor result = null;
            foreach (RotateAnchor ele in rotateAnchorCollection)
            {
                if (ele.HitTest(mousePosition))
                {
                    result = ele;
                }
            }
            return result;
        }

        //Os primeiros dois parâmetros são o coordenadas X e Y do ponto central (a origem em torno do qual o segundo ponto vai ser rodado). 
         // Os próximos dois parâmetros são as coordenadas do ponto que vamos ser rotativa. O último parâmetro é o ângulo, em graus.
        private SharpDX.Vector2 Vector2DRotate(SharpDX.Vector2 centerPoint, SharpDX.Vector2 pointToRotate, float angle)
        {
            float cx = centerPoint.X;
            float cy = centerPoint.Y;

            float x = pointToRotate.X;
            float y = pointToRotate.Y;

            float radians = ((float)Math.PI / 180) * angle;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            float nx = (cos * (x - cx)) + (sin * (y - cy)) + cx;
            float ny = (cos * (y - cy)) - (sin * (x - cx)) + cy;
            return new SharpDX.Vector2(nx, ny);
        }*/

        //Adiciona elemento no canvas
        public void AddElement(Element element)
        {
            renderBackend.ElementCollection.Add(element);
            UpdateDraw();
        }
        //Deleta elemento do canvas
        public void RemoveElement(Element element)
        {
            renderBackend.ElementCollection.Remove(element);
            UpdateDraw();
        }
        //Remove todos elementos do canvas
        public void ClearElement()
        {
            renderBackend.ElementCollection.Clear();
            UpdateDraw();
        }

        //Desenho na tela
        private void Draw()
        {
            //Verifica se é nescessario redesenhar a tela se for true desenha 
            if (redraw)
            {
                renderBackend.Draw();
                 /*
                 foreach (Element element in ElementCollection)
                 {
                     element.SetFactory(Direct2D1Factory);
                     element.SetRenderTarget(Direct2D1RenderTarget);
                     element.Draw();

                     if (ReferenceEquals(element, ElementSelected))
                     {
                         foreach (DragAnchor dragAnchor in dragAnchorCollection)
                         {
                             dragAnchor.SetFactory(Direct2D1Factory);
                             dragAnchor.SetRenderTarget(Direct2D1RenderTarget);
                         }
                         dragAnchorCollection[0].Draw(element.X, element.Y, element.Rotation, element.GetCenterPoint());
                         dragAnchorCollection[1].Draw(element.X + element.Width / 2, element.Y, element.Rotation, element.GetCenterPoint());
                         dragAnchorCollection[2].Draw(element.X + element.Width, element.Y, element.Rotation, element.GetCenterPoint());
                         dragAnchorCollection[3].Draw(element.X, element.Y + element.Height / 2, element.Rotation, element.GetCenterPoint());
                         dragAnchorCollection[4].Draw(element.X + element.Width, element.Y + element.Height / 2, element.Rotation, element.GetCenterPoint());
                         dragAnchorCollection[5].Draw(element.X, element.Y + element.Height, element.Rotation, element.GetCenterPoint());
                         dragAnchorCollection[6].Draw(element.X + element.Width / 2, element.Y + element.Height, element.Rotation, element.GetCenterPoint());
                         dragAnchorCollection[7].Draw(element.X + element.Width, element.Y + element.Height, element.Rotation, element.GetCenterPoint());

                         rotateAnchorCollection[0].SetFactory(Direct2D1Factory);
                         rotateAnchorCollection[0].SetRenderTarget(Direct2D1RenderTarget);
                         rotateAnchorCollection[0].Draw(element.X - 20, element.Y - 20, element.Rotation, element.GetCenterPoint());


                         SolidColorBrush fillColor = new SolidColorBrush(Direct2D1RenderTarget, Util.ARGBColorToRawColor4(200, 204, 0, 0));
                         Direct2D1RenderTarget.Transform = SharpDX.Matrix3x2.Rotation((float)(float)(element.Rotation), new SharpDX.Vector2((float)(element.X + (element.Width / 2)), (float)(element.Y + (element.Height / 2))));
                         Direct2D1RenderTarget.DrawRectangle(new RawRectangleF((float)element.X, (float)element.Y, (float)(element.X + element.Width), (float)(element.Y + element.Height)), fillColor, 2);
                         Direct2D1RenderTarget.Transform = SharpDX.Matrix3x2.Identity;
                         Util.SafeDispose(ref fillColor);
                     }
                 }
                 */


                 redraw = false;
            }
        }

        //Limpa a tela
        public void Clear()
        {
            renderBackend.Clear();
        }

        //Redesenha
        public void UpdateDraw()
        {
            renderBackend.UpdateDraw();
        }

        //Atualiza a tela apos Window Resize
        public void UpdateResize()
        {
            renderBackend.UpdateDrawOnResize(this.ActualWidth,this.ActualHeight);
        }       

        public void Release()
        {
            actHook.Stop();
            actHook = null;
            renderBackend.Release();
        }
    }
}