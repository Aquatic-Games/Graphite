using Graphite.Core;

namespace Graphite;

public record struct SwapchainInfo
{
    public Surface Surface;

    public Format Format;

    public Size2D Size;

    public PresentMode PresentMode;

    public uint NumBuffers;

    public SwapchainInfo(Surface surface, Format format, Size2D size, PresentMode presentMode, uint numBuffers)
    {
        Surface = surface;
        Format = format;
        Size = size;
        PresentMode = presentMode;
        NumBuffers = numBuffers;
    }
}