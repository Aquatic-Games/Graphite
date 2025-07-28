using System.Drawing;
using System.Runtime.CompilerServices;
using Graphite;
using Graphite.Core;
using Graphite.Vulkan;
using SDL3;
using Buffer = Graphite.Buffer;

GraphiteLog.LogMessage += (severity, type, message, _, _) =>
{
    if (severity == GraphiteLog.Severity.Error)
        throw new Exception(message);

    Console.WriteLine($"{severity} - {type}: {message}");
};

if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Events))
    throw new Exception($"Failed to initialize SDL: {SDL.GetError()}");

const int width = 1280;
const int height = 720;

IntPtr window = SDL.CreateWindow("Graphite.Tests", width, height, 0);
if (window == IntPtr.Zero)
    throw new Exception($"Failed to create window: {SDL.GetError()}");

Instance instance = Instance.Create(new InstanceInfo("Graphite.Tests", true));
Console.WriteLine($"Adapters: {string.Join(", ", instance.EnumerateAdapters())}");

uint properties = SDL.GetWindowProperties(window);
SurfaceInfo surfaceInfo;

if (OperatingSystem.IsWindows())
{
    nint hinstance = SDL.GetPointerProperty(properties, SDL.Props.WindowWin32InstancePointer, 0);
    nint hwnd = SDL.GetPointerProperty(properties, SDL.Props.WindowWin32HWNDPointer, 0);
    surfaceInfo = SurfaceInfo.Windows(hinstance, hwnd);
}
else if (OperatingSystem.IsLinux())
{
    if (SDL.GetCurrentVideoDriver() == "wayland")
    {
        nint display = SDL.GetPointerProperty(properties, SDL.Props.WindowWaylandDisplayPointer, 0);
        nint wsurface = SDL.GetPointerProperty(properties, SDL.Props.WindowWaylandSurfacePointer, 0);
        surfaceInfo = SurfaceInfo.Wayland(display, wsurface);
    }
    else if (SDL.GetCurrentVideoDriver() == "x11")
    {
        nint display = SDL.GetPointerProperty(properties, SDL.Props.WindowX11DisplayPointer, 0);
        long xwindow = SDL.GetNumberProperty(properties, SDL.Props.WindowX11WindowNumber, 0);
        surfaceInfo = SurfaceInfo.Xlib(display, (nint) xwindow);
    }
    else
        throw new PlatformNotSupportedException();
}
else
    throw new PlatformNotSupportedException();

Surface surface = instance.CreateSurface(in surfaceInfo);
Device device = instance.CreateDevice(surface);
CommandList cl = device.CreateCommandList();
Swapchain swapchain =
    device.CreateSwapchain(new SwapchainInfo(surface, Format.B8G8R8A8_UNorm, new Size2D(width, height),
        PresentMode.Fifo, 2));

ReadOnlySpan<float> vertices =
[
    -0.5f, -0.5f, 1.0f, 0.0f, 0.0f,
    -0.5f, +0.5f, 0.0f, 1.0f, 0.0f,
    +0.5f, +0.5f, 0.0f, 0.0f, 1.0f,
    +0.5f, -0.5f, 0.0f, 0.0f, 0.0f
];

ReadOnlySpan<ushort> indices =
[
    0, 1, 3,
    1, 2, 3
];

uint vertexSize = (uint) vertices.Length * sizeof(float);
uint indexSize = (uint) indices.Length * sizeof(ushort);

Buffer vertexBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.VertexBuffer, vertexSize));
Buffer indexBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.IndexBuffer, indexSize));

Buffer transferBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.TransferBuffer, vertexSize + indexSize));
nint mappedBuffer = device.MapBuffer(transferBuffer);
unsafe
{
    fixed (float* pVertices = vertices)
        Unsafe.CopyBlock((byte*) mappedBuffer, pVertices, vertexSize);
    fixed (ushort* pIndices = indices)
        Unsafe.CopyBlock((byte*) mappedBuffer + vertexSize, pIndices, indexSize);
}
device.UnmapBuffer(transferBuffer);

cl.Begin();
cl.CopyBufferToBuffer(transferBuffer, 0, vertexBuffer, 0);
cl.CopyBufferToBuffer(transferBuffer, vertexSize, indexBuffer, 0);
cl.End();
device.ExecuteCommandList(cl);

transferBuffer.Dispose();

byte[] vShader = File.ReadAllBytes("Shader_v.spv");
byte[] pShader = File.ReadAllBytes("Shader_p.spv");

ShaderModule vertexShader = device.CreateShaderModule(vShader, "VSMain");
ShaderModule pixelShader = device.CreateShaderModule(pShader, "PSMain");

Pipeline pipeline =
    device.CreateGraphicsPipeline(new GraphicsPipelineInfo(vertexShader, pixelShader,
        [new ColorTargetInfo(Format.B8G8R8A8_UNorm)]));

pixelShader.Dispose();
vertexShader.Dispose();

bool alive = true;
while (alive)
{
    while (SDL.PollEvent(out SDL.Event winEvent))
    {
        switch ((SDL.EventType) winEvent.Type)
        {
            case SDL.EventType.WindowCloseRequested:
                alive = false;
                break;
        }
    }

    Texture texture = swapchain.GetNextTexture();
    
    cl.Begin();
    cl.BeginRenderPass([new ColorAttachmentInfo(texture, new ColorF(Color.CornflowerBlue))]);
    
    cl.SetGraphicsPipeline(pipeline);
    cl.Draw(6);
    
    cl.EndRenderPass();
    cl.End();
    
    device.ExecuteCommandList(cl);
    swapchain.Present();
}

pipeline.Dispose();
indexBuffer.Dispose();
vertexBuffer.Dispose();
swapchain.Dispose();
cl.Dispose();
device.Dispose();
surface.Dispose();
instance.Dispose();
SDL.DestroyWindow(window);
SDL.Quit();