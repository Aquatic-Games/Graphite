using Graphite.Core;

namespace Graphite;

public record struct SwapchainInfo
{
    public Surface Surface;

    public Size2D Size;

    public Format Format;

    public PresentMode PresentMode;

    public uint NumBuffers;

    public SwapchainInfo(Surface surface, Size2D size, Format format, PresentMode presentMode, uint numBuffers)
    {
        Surface = surface;
        Size = size;
        Format = format;
        PresentMode = presentMode;
        NumBuffers = numBuffers;
    }
}