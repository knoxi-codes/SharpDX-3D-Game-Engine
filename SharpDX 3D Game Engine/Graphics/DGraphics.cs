using SharpDX_3D_Game_Engine.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX_3D_Game_Engine.Graphics
{
    public class DGraphics
    {
        private DDX11 D3D { get; set; }
        public bool Initialize(DSystemConfig Config, IntPtr windowHandle)
        {
            try 
            {
                D3D = new DDX11();

                if (!D3D.Initialize(Config, windowHandle))
                    return false;

                return true;
            }
            catch 
            {
                return true;
            }
        }

        public void Shutdown()
        {
            D3D?.ShutDown();
            D3D = null;
        }

        public bool Frame()
        {
            return Render();
        }

        public bool Render()
        {
            D3D.BeginScene(0.5f, 0.5f, 0.5f, 1.0f);

            D3D.EndScene();
            return true;
        }
    }
}