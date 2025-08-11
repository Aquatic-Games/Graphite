using Graphite.Core;

namespace Graphite;

public record struct TextureInfo
{
    public TextureType Type;

    public Format Format;

    public Size3D Size;

    public uint MipLevels;

    public uint ArraySize;

    public TextureUsage Usage;

    public TextureInfo(TextureType type, Format format, Size3D size, uint mipLevels, uint arraySize, TextureUsage usage)
    {
        Type = type;
        Format = format;
        Size = size;
        MipLevels = mipLevels;
        ArraySize = arraySize;
        Usage = usage;
    }

    public static TextureInfo Texture2D(Format format, Size2D size, uint mipLevels, TextureUsage usage)
        => new TextureInfo(TextureType.Texture2D, format, (Size3D) size, mipLevels, 1, usage);
}