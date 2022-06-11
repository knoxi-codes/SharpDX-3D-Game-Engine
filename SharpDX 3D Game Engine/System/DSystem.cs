using SharpDX.Windows;
using SharpDX_3D_Game_Engine.Graphics;
using SharpDX_3D_Game_Engine.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDX_3D_Game_Engine.System
{
    public class DSystem
    {
        private RenderForm Renderform;
        public DSystemConfig Config;
        public DGraphics Graphics;
        public DInput Input;

        public DSystem() { }

        public static void StartRenderForm(string title, int width, int height, bool vSync, bool fullScreen = true, int testTimeSeconds = 0)
        { 
            DSystem system = new DSystem();
            system.Initialize(title, width, height, vSync, fullScreen, testTimeSeconds);
            system.RunRenderForm();
        }

        public virtual bool Initialize(string title, int width, int height, bool vSync, bool fullScreen, int testTimeSeconds)
        {
            bool result = false;

            if (Config == null)
                Config = new DSystemConfig(title, width, height, fullScreen, vSync);

            InitializeWindows(title);

            Renderform.BackColor = Color.Black;

            if (Graphics == null)
            { 
                Graphics = new DGraphics();
                result = Graphics.Initialize(Config);
            }
            if (Input == null)
            { 
                Input = new DInput();
                Input.Initialize();
            }

            return result;
        }

        private void InitializeWindows(string title)
        { 
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;

            Renderform = new RenderForm(title)
            {
                ClientSize = new Size(Config.Width, Config.Height),
            };

            Renderform.Show();
            Renderform.Location = new Point((width / 2) - (Config.Width / 2), (height / 2) - (Config.Height / 2));
        }

        private void RunRenderForm()
        {
            Renderform.KeyDown += (s, e) => Input.KeyDown(e.KeyCode);
            Renderform.KeyUp += (s, e) => Input.KeyUp(e.KeyCode);

            RenderLoop.Run(Renderform, () =>
            {
                if (!Frame())
                    Shutdown();
            });
        }

        public bool Frame()
        {
            if (Input.IsKeyDown(Keys.Escape))
                return false;
            return Graphics.Frame();
        }

        public void Shutdown()
        { 
            Renderform?.Dispose();
            Renderform = null;

            Graphics?.Shutdown();
            Graphics = null;
        }
    }
}
