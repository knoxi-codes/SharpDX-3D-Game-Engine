using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX_3D_Game_Engine.System;

namespace SharpDX_3D_Game_Engine.Graphics
{
    public class DDX11
    {
        private bool VerticalSyncEnabled { get; set; }
        public int VideoCardMemory { get; private set; }
        public string VideoCardDescription { get; private set; }
        private SwapChain SwapChain { get; set; }
        public SharpDX.Direct3D11.Device Device { get; private set; }
        public DeviceContext DeviceContext { get; private set; }
        private RenderTargetView RenderTargetView { get; set; }
        private Texture2D DepthStencilBuffer { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        private DepthStencilView DepthStencilView { get; set; }
        private RasterizerState RasterizerState { get; set; }

        public DDX11() { }

        public bool Initialize(DSystemConfig config, IntPtr windowHandle)
        {
            try 
            {
                VerticalSyncEnabled = DSystemConfig.VerticalSyncEnabled;

                var factory = new Factory1();

                var adapter = factory.GetAdapter1(0);

                var monitor = adapter.GetOutput(0);

                var modes = monitor.GetDisplayModeList(Format.R8G8B8A8_UNorm, DisplayModeEnumerationFlags.Interlaced);

                var rational = new Rational(0, 1);

                if (VerticalSyncEnabled)
                {
                    foreach (var mode in modes)
                    {
                        if (mode.Width == config.Width && mode.Height == config.Height)
                        {
                            rational = new Rational(mode.RefreshRate.Numerator, mode.RefreshRate.Denominator);
                            break;
                        }
                    }
                }

                var adapterDescription = adapter.Description;
                VideoCardMemory = 1024;
                VideoCardDescription = adapterDescription.Description.Trim('\0');

                monitor.Dispose();

                adapter.Dispose();

                factory.Dispose();

                var swapChainDesc = new SwapChainDescription()
                { 
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(config.Width, config.Height, rational, Format.R8G8B8A8_UNorm),
                    Usage = Usage.RenderTargetOutput,
                    OutputHandle = windowHandle,
                    SampleDescription = new SampleDescription(1, 0),
                    IsWindowed = !DSystemConfig.FullScreen,
                    Flags = SwapChainFlags.None,
                    SwapEffect = SwapEffect.Discard
                };

                SharpDX.Direct3D11.Device device;
                SwapChain swapChain;
                SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out device, out swapChain);

                Device = device;
                SwapChain = swapChain;
                DeviceContext = Device.ImmediateContext;

                var backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);

                RenderTargetView = new RenderTargetView(device, backBuffer);

                backBuffer.Dispose();

                var depthBufferDesc = new Texture2DDescription()
                { 
                    Width = config.Width,
                    Height = config.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.D24_UNorm_S8_UInt,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                };

                DepthStencilBuffer = new Texture2D(device, depthBufferDesc);

                var depthStencilDesc = new DepthStencilStateDescription()
                { 
                    IsDepthEnabled = true,
                    DepthWriteMask = DepthWriteMask.All,
                    DepthComparison = Comparison.Less,
                    IsStencilEnabled = true,
                    StencilReadMask = 0xFF,
                    StencilWriteMask = 0xFF,
                    FrontFace = new DepthStencilOperationDescription()
                    { 
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Increment,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    },
                    BackFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Decrement,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    }
                };

                DepthStencilState = new DepthStencilState(Device, depthStencilDesc);

                DeviceContext.OutputMerger.SetDepthStencilState(DepthStencilState, 1);

                var depthStencilViewDesc = new DepthStencilViewDescription()
                {
                    Format = Format.D24_UNorm_S8_UInt,
                    Dimension = DepthStencilViewDimension.Texture2D,
                    Texture2D = new DepthStencilViewDescription.Texture2DResource()
                    { 
                        MipSlice = 0
                    }
                };

                DepthStencilView = new DepthStencilView(Device, DepthStencilBuffer, depthStencilViewDesc);

                DeviceContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);

                var rasterDesc = new RasterizerStateDescription()
                {
                    IsAntialiasedLineEnabled = false,
                    CullMode = CullMode.Back,
                    DepthBias = 0,
                    DepthBiasClamp = .0f,
                    IsDepthClipEnabled = true,
                    FillMode = FillMode.Solid,
                    IsFrontCounterClockwise = false,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = .0f
                };

                RasterizerState = new RasterizerState(Device, rasterDesc);

                DeviceContext.Rasterizer.State = RasterizerState;

                DeviceContext.Rasterizer.SetViewport(0, 0, config.Width, config.Height, 0, 1);

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public void ShutDown()
        {
            SwapChain?.SetFullscreenState(false, null);

            RasterizerState?.Dispose();
            RasterizerState = null;
            DepthStencilView?.Dispose();
            DepthStencilView = null;
            DepthStencilState?.Dispose();
            DepthStencilState = null;
            DepthStencilBuffer?.Dispose();
            DepthStencilBuffer = null;
            RenderTargetView?.Dispose();
            RenderTargetView = null;
            DeviceContext?.Dispose();
            DeviceContext = null;
            Device?.Dispose();
            Device = null;
            SwapChain?.Dispose();
            SwapChain = null;
        }

        public void BeginScene(float red, float green, float blue, float alpha)
        {
            DeviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1, 0);

            DeviceContext.ClearRenderTargetView(RenderTargetView, new Color4(red, green, blue, alpha));
        }
        public void EndScene()
        {
            if (VerticalSyncEnabled)
                SwapChain.Present(1, 0);
            else
                SwapChain.Present(0, 0);
        }
    }
}
