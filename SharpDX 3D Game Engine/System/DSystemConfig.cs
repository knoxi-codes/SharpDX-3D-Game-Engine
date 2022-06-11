using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX_3D_Game_Engine.System
{
    public class DSystemConfig
    {
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public static FormBorderStyle BorderStyle { get; set; }
        public static bool FullScreen { get; set; }

        public DSystemConfig(string title, int width, int height, bool fullScreen, bool vSync)
        { 
            FullScreen = fullScreen;
            Title = title;

            if (!FullScreen)
            {
                Width = width;
                Height = height;
            }
            else 
            {
                Width = Screen.PrimaryScreen.Bounds.Width;
                Height = Screen.PrimaryScreen.Bounds.Height;
            }
        }
    }
}
