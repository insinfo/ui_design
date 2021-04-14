using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UIDesign
{
    public abstract class RenderBackend 
    {
        public List<Element> ElementCollection;

        public abstract void Initialize(Image imageControl);

        public abstract void Draw();

        public abstract void Clear();

        public abstract void UpdateDraw();

        public abstract void UpdateDrawOnResize(double actualWidth, double actualHeight);

        internal abstract void RenderEngineInit();

        internal abstract void RenderEngineConfig();

        internal abstract void RenderImageSourceInit();

        internal abstract void RenderImageSourceConfig();

        public abstract void Release();
      
    }
}
