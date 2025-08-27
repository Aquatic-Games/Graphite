using Graphite.Core;

namespace Graphite;

public struct SamplerInfo
{
    public const uint AnisotropicMax = 16;
    
    public Filter MinFilter;

    public Filter MagFilter;

    public Filter MipFilter;

    public AddressMode AddressU;

    public AddressMode AddressV;

    public AddressMode AddressW;

    public uint MaxAnisotropy;

    public uint MinLod;

    public uint MaxLod;
    
    //public ColorF BorderColor;

    public SamplerInfo(Filter minFilter, Filter magFilter, Filter mipFilter, AddressMode addressU, AddressMode addressV,
        AddressMode addressW, uint maxAnisotropy = 0, uint minLod = 0, uint maxLod = 1000/*, ColorF borderColor = default*/)
    {
        MinFilter = minFilter;
        MagFilter = magFilter;
        MipFilter = mipFilter;
        AddressU = addressU;
        AddressV = addressV;
        AddressW = addressW;
        MaxAnisotropy = maxAnisotropy;
        MinLod = minLod;
        MaxLod = maxLod;
        //BorderColor = borderColor;
    }

    public SamplerInfo(Filter filter, AddressMode addressMode, uint maxAnisotropy = 0, uint minLod = 0,
        uint maxLod = 1000/*, ColorF borderColor = default*/) : this(filter, filter, filter, addressMode, addressMode,
        addressMode, maxAnisotropy, minLod, maxLod/*, borderColor*/) { }

    public static SamplerInfo PointWrap => new SamplerInfo(Filter.Point, AddressMode.Wrap);

    public static SamplerInfo PointClamp => new SamplerInfo(Filter.Point, AddressMode.ClampToEdge);

    public static SamplerInfo LinearWrap => new SamplerInfo(Filter.Linear, AddressMode.Wrap);

    public static SamplerInfo LinearClamp => new SamplerInfo(Filter.Linear, AddressMode.ClampToEdge);

    public static SamplerInfo Anisotropic => new SamplerInfo(Filter.Linear, AddressMode.Wrap, AnisotropicMax);
}