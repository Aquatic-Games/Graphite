using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Graphite;
using Graphite.Core;
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

IntPtr window = SDL.CreateWindow("Graphite.Tests", width, height, SDL.WindowFlags.Resizable | SDL.WindowFlags.Vulkan);
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
    if (instance.Backend == Backend.D3D11)
        surfaceInfo = new SurfaceInfo(SurfaceType.Win32, 0, window);
    else if (SDL.GetCurrentVideoDriver() == "wayland")
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

Buffer vertexBuffer = device.CreateBuffer(BufferUsage.VertexBuffer, vertices);
Buffer indexBuffer = device.CreateBuffer(BufferUsage.IndexBuffer, indices);
Buffer constantBuffer = device.CreateBuffer(BufferUsage.ConstantBuffer | BufferUsage.MapWrite, Matrix4x4.CreateRotationZ(1));

/*uint vertexSize = (uint) vertices.Length * sizeof(float);
uint indexSize = (uint) indices.Length * sizeof(ushort);
uint cBufferSize = 64;

Buffer vertexBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.VertexBuffer, vertexSize));
Buffer indexBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.IndexBuffer, indexSize));
Buffer constantBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.ConstantBuffer | BufferUsage.MapWrite, cBufferSize));

Buffer transferBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.TransferBuffer, vertexSize + indexSize + cBufferSize));
nint mappedBuffer = device.MapBuffer(transferBuffer);
unsafe
{
    fixed (float* pVertices = vertices)
        Unsafe.CopyBlock((byte*) mappedBuffer, pVertices, vertexSize);
    fixed (ushort* pIndices = indices)
        Unsafe.CopyBlock((byte*) mappedBuffer + vertexSize, pIndices, indexSize);
    Matrix4x4 identity = Matrix4x4.Identity;
    Unsafe.CopyBlock((byte*) mappedBuffer + vertexSize + indexSize, Unsafe.AsPointer(ref identity), cBufferSize);
}
device.UnmapBuffer(transferBuffer);

cl.Begin();
cl.CopyBufferToBuffer(transferBuffer, 0, vertexBuffer, 0);
cl.CopyBufferToBuffer(transferBuffer, vertexSize, indexBuffer, 0);
cl.CopyBufferToBuffer(transferBuffer, vertexSize + indexSize, constantBuffer, 0);
cl.End();
device.ExecuteCommandList(cl);

transferBuffer.Dispose();*/

byte[] vShader = File.ReadAllBytes("Shader_v.fxc");
byte[] pShader = File.ReadAllBytes("Shader_p.fxc");

ShaderModule vertexShader = device.CreateShaderModule(vShader, "VSMain",
    new ShaderMappingInfo([
        new VertexInputMapping(Semantic.Position, 0), new VertexInputMapping(Semantic.Color, 0)
    ]));
ShaderModule pixelShader = device.CreateShaderModule(pShader, "PSMain");

/*DescriptorLayout transformLayout =
    device.CreateDescriptorLayout(new DescriptorBinding(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex));
DescriptorSet transformSet = device.CreateDescriptorSet(transformLayout, new Descriptor(0, DescriptorType.ConstantBuffer, constantBuffer));*/

Pipeline pipeline = device.CreateGraphicsPipeline(new GraphicsPipelineInfo
{
    VertexShader = vertexShader,
    PixelShader = pixelShader,
    ColorTargets = [new ColorTargetInfo(Format.B8G8R8A8_UNorm)],
    InputLayout =
    [
        new InputElementDescription(Format.R32G32_Float, 0, 0, 0),
        new InputElementDescription(Format.R32G32B32_Float, 8, 1, 0)
    ],
    //Descriptors = [transformLayout]
});

pixelShader.Dispose();
vertexShader.Dispose();

float value = 0;

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

    /*nint map = device.MapBuffer(constantBuffer);
    Matrix4x4 matrix = Matrix4x4.CreateRotationZ(value);
    unsafe { Unsafe.CopyBlock((void*) map, Unsafe.AsPointer(ref matrix), 64); }
    device.UnmapBuffer(constantBuffer);
    value += 0.01f;
    if (value >= float.Pi * 2)
        value -= float.Pi * 2;*/
    
    cl.Begin();
    cl.BeginRenderPass([new ColorAttachmentInfo(texture, new ColorF(Color.CornflowerBlue))]);
    
    cl.SetGraphicsPipeline(pipeline);
    //cl.SetDescriptorSet(0, pipeline, transformSet);
    cl.SetVertexBuffer(0, vertexBuffer, 5 * sizeof(float));
    cl.SetIndexBuffer(indexBuffer, Format.R16_UInt);
    cl.DrawIndexed(6);
    //cl.Draw(6);
    
    cl.EndRenderPass();
    cl.End();
    
    device.ExecuteCommandList(cl);
    swapchain.Present();
}

pipeline.Dispose();
/*transformSet.Dispose();
transformLayout.Dispose();*/
constantBuffer.Dispose();
indexBuffer.Dispose();
vertexBuffer.Dispose();
swapchain.Dispose();
cl.Dispose();
device.Dispose();
surface.Dispose();
instance.Dispose();
SDL.DestroyWindow(window);
SDL.Quit();