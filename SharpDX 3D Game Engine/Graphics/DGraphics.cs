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
        public bool Initialize(DSystemConfig Config)
        {
            return true;
        }

        public void Shutdown()
        { 
            
        }

        public bool Frame()
        {
            return true;
        }

        public bool Render()
        {
            return true;
        }
    }
}
