using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Graphite;
using Graphite.Core;
using Graphite.D3D11;
using Graphite.OpenGL;
using Graphite.ShaderTools;
using Graphite.Vulkan;
using SDL3;
using StbImageSharp;
using Buffer = Graphite.Buffer;

GraphiteLog.LogMessage += (severity, type, message, _, _) =>
{
    Console.WriteLine($"{severity} - {type}: {message}");
};

if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Events))
    throw new Exception($"Failed to initialize SDL: {SDL.GetError()}");

Instance.RegisterBackend<OpenGLBackend>();
Instance.RegisterBackend<D3D11Backend>();
Instance.RegisterBackend<VulkanBackend>();

const int width = 800;
const int height = 600;

SDL.GLSetAttribute(SDL.GLAttr.ContextMajorVersion, OpenGLBackend.MajorVersion);
SDL.GLSetAttribute(SDL.GLAttr.ContextMinorVersion, OpenGLBackend.MinorVersion);
SDL.GLSetAttribute(SDL.GLAttr.ContextProfileMask, (int) SDL.GLProfile.Core);

IntPtr window = SDL.CreateWindow($"Graphite.SimpleTest", width, height, SDL.WindowFlags.Resizable | SDL.WindowFlags.OpenGL);
if (window == IntPtr.Zero)
    throw new Exception($"Failed to create window: {SDL.GetError()}");

IntPtr context = SDL.GLCreateContext(window);
SDL.GLMakeCurrent(window, context);

OpenGLBackend.Context = new GLContext(s => Marshal.GetFunctionPointerForDelegate(SDL.GLGetProcAddress(s)), i =>
{
    SDL.GLSetSwapInterval(i);
    SDL.GLSwapWindow(window);
});

Instance instance = Instance.Create(new InstanceInfo("Graphite.SimpleTest", true));
SDL.SetWindowTitle(window, SDL.GetWindowTitle(window) + $" - {instance.BackendName}");

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

/*ReadOnlySpan<float> vertices =
[
    -0.5f, -0.5f, 0.0f, 1.0f,
    -0.5f, +0.5f, 0.0f, 0.0f,
    +0.5f, +0.5f, 1.0f, 0.0f,
    +0.5f, -0.5f, 1.0f, 1.0f,
];

ReadOnlySpan<ushort> indices =
[
    0, 1, 3,
    1, 2, 3
];

ImageResult result0 = ImageResult.FromMemory(File.ReadAllBytes("DEBUG.png"), ColorComponents.RedGreenBlueAlpha);
ImageResult result1 = ImageResult.FromMemory(File.ReadAllBytes("Bagel.png"), ColorComponents.RedGreenBlueAlpha);

TextureInfo textureInfo = new()
{
    Type = TextureType.Texture2D,
    Format = Format.R8G8B8A8_UNorm,
    Size = new Size3D((uint) result0.Width, (uint) result0.Height),
    Usage = TextureUsage.ShaderResource | TextureUsage.GenerateMips,
    ArraySize = 1
};

Texture texture0 = device.CreateTexture(in textureInfo, result0.Data);
Sampler sampler = device.CreateSampler(SamplerInfo.LinearClamp);

Buffer vertexBuffer = device.CreateBuffer(BufferUsage.VertexBuffer, vertices);
Buffer indexBuffer = device.CreateBuffer(BufferUsage.IndexBuffer, indices);
Buffer constantBuffer = device.CreateBuffer(BufferUsage.ConstantBuffer | BufferUsage.MapWrite, Matrix4x4.CreateRotationZ(1));

textureInfo.Size = new Size3D((uint) result1.Width, (uint) result1.Height);
Texture texture1 = device.CreateTexture(in textureInfo, result1.Data);

cl.Begin();
cl.GenerateMipmaps(texture0);
cl.GenerateMipmaps(texture1);
cl.End();
device.ExecuteCommandList(cl);*/

/*uint vertexSize = (uint) vertices.Length * sizeof(float);
uint indexSize = (uint) indices.Length * sizeof(ushort);
uint cBufferSize = 64;
uint textureSize = (uint) (result1.Width * result1.Height * 4); // 32bpp

Buffer vertexBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.VertexBuffer, vertexSize));
Buffer indexBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.IndexBuffer, indexSize));
Buffer constantBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.ConstantBuffer | BufferUsage.MapWrite, cBufferSize));

textureInfo.Size = new Size3D((uint) result1.Width, (uint) result1.Height);
Texture texture1 = device.CreateTexture(in textureInfo);

Buffer transferBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.TransferBuffer, vertexSize + indexSize + cBufferSize + textureSize));
nint mappedBuffer = device.MapBuffer(transferBuffer);
unsafe
{
    fixed (float* pVertices = vertices)
        Unsafe.CopyBlock((byte*) mappedBuffer, pVertices, vertexSize);
    fixed (ushort* pIndices = indices)
        Unsafe.CopyBlock((byte*) mappedBuffer + vertexSize, pIndices, indexSize);
    
    Matrix4x4 identity = Matrix4x4.Identity;
    Unsafe.CopyBlock((byte*) mappedBuffer + vertexSize + indexSize, Unsafe.AsPointer(ref identity), cBufferSize);

    fixed (byte* pData = result1.Data)
        Unsafe.CopyBlock((byte*) mappedBuffer + vertexSize + indexSize + cBufferSize, pData, textureSize);
}
device.UnmapBuffer(transferBuffer);

cl.Begin();
cl.CopyBufferToBuffer(transferBuffer, 0, vertexBuffer, 0);
cl.CopyBufferToBuffer(transferBuffer, vertexSize, indexBuffer, 0);
cl.CopyBufferToBuffer(transferBuffer, vertexSize + indexSize, constantBuffer, 0);
cl.CopyBufferToTexture(transferBuffer, vertexSize + indexSize + cBufferSize, texture1);
cl.GenerateMipmaps(texture0);
cl.GenerateMipmaps(texture1);
cl.End();
device.ExecuteCommandList(cl);

transferBuffer.Dispose();*/

/*string shader = File.ReadAllText("Shader.hlsl");

ShaderModule vertexShader = device.CreateShaderModuleFromHLSL(ShaderStage.Vertex, shader, "VSMain");
ShaderModule pixelShader = device.CreateShaderModuleFromHLSL(ShaderStage.Pixel, shader, "PSMain");

DescriptorLayout textureLayout = device.CreateDescriptorLayout(new DescriptorLayoutInfo
{
    Bindings =
    [
        new DescriptorBinding(0, DescriptorType.Texture, ShaderStage.Pixel),
        new DescriptorBinding(1, DescriptorType.Texture, ShaderStage.Pixel)
    ]
});
DescriptorSet textureSet = device.CreateDescriptorSet(textureLayout,
    new Descriptor(0, DescriptorType.Texture, texture: texture0, sampler: sampler),
    new Descriptor(1, DescriptorType.Texture, texture: texture1, sampler: sampler));

DescriptorLayout transformLayout = device.CreateDescriptorLayout(new DescriptorLayoutInfo
{
    Bindings = [new DescriptorBinding(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex)],
    PushDescriptor = true
});

Pipeline pipeline = device.CreateGraphicsPipeline(new GraphicsPipelineInfo
{
    VertexShader = vertexShader,
    PixelShader = pixelShader,
    ColorTargets = [new ColorTargetInfo(Format.B8G8R8A8_UNorm)],
    InputLayout =
    [
        new InputElementDescription(Format.R32G32_Float, 0, 0, 0),
        new InputElementDescription(Format.R32G32_Float, 8, 1, 0)
    ],
    Descriptors = [textureLayout, transformLayout]
});

pixelShader.Dispose();
vertexShader.Dispose();*/

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

    Texture swapchainTexture = swapchain.GetNextTexture();

    /*nint map = device.MapBuffer(constantBuffer);
    Matrix4x4 matrix = Matrix4x4.CreateRotationZ(value);
    unsafe { Unsafe.CopyBlock((void*) map, Unsafe.AsPointer(ref matrix), 64); }
    device.UnmapBuffer(constantBuffer);
    value += 0.01f;
    if (value >= float.Pi * 2)
        value -= float.Pi * 2;*/
    
    cl.Begin();
    cl.BeginRenderPass([new ColorAttachmentInfo(swapchainTexture, new ColorF(Color.CornflowerBlue))]);
    
    /*cl.SetGraphicsPipeline(pipeline);
    
    cl.SetDescriptorSet(0, pipeline, textureSet);
    cl.PushDescriptors(1, pipeline, new Descriptor(0, DescriptorType.ConstantBuffer, constantBuffer));
    
    cl.SetVertexBuffer(0, vertexBuffer, 4 * sizeof(float));
    cl.SetIndexBuffer(indexBuffer, Format.R16_UInt);
    
    cl.DrawIndexed(6);*/
    //cl.Draw(6);
    
    cl.EndRenderPass();
    cl.End();
    
    device.ExecuteCommandList(cl);
    swapchain.Present();
}

/*pipeline.Dispose();
transformLayout.Dispose();
textureSet.Dispose();
textureLayout.Dispose();
sampler.Dispose();
texture1.Dispose();
texture0.Dispose();
constantBuffer.Dispose();
indexBuffer.Dispose();
vertexBuffer.Dispose();*/
swapchain.Dispose();
cl.Dispose();
device.Dispose();
surface.Dispose();
instance.Dispose();
SDL.DestroyWindow(window);
SDL.Quit();