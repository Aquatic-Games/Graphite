namespace Graphite;

public struct ColorTargetInfo
{
    public Format Format;

    public BlendStateDescription BlendState;

    public ColorTargetInfo(Format format, BlendStateDescription blendState = default)
    {
        Format = format;
        BlendState = blendState;
    }
}