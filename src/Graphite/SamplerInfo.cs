using Graphite.Core;

namespace Graphite;

public struct SamplerInfo
{
    /// <summary>
    /// The maximum supported anisotropic level.
    /// </summary>
    public const uint MaxAnisotropicLevel = 16;
    
    /// <summary>
    /// The minification filter.
    /// </summary>
    public Filter MinFilter;

    /// <summary>
    /// The magnification filter.
    /// </summary>
    public Filter MagFilter;

    /// <summary>
    /// The mipmap filter.
    /// </summary>
    public Filter MipFilter;

    /// <summary>
    /// How a U texture coordinate that is outside the 0-1 range should be resolved.
    /// </summary>
    public AddressMode AddressU;

    /// <summary>
    /// How a V texture coordinate that is outside the 0-1 range should be resolved.
    /// </summary>
    public AddressMode AddressV;

    /// <summary>
    /// How a W texture coordinate that is outside the 0-1 range should be resolved.
    /// </summary>
    public AddressMode AddressW;

    /// <summary>
    /// The max anisotropy level. Set to 0 to disable.
    /// </summary>
    public uint MaxAnisotropy;

    /// <summary>
    /// The minimum mip level of detail (LOD), where 0 is the maximum detail.
    /// </summary>
    public float MinLod;

    /// <summary>
    /// The maximum mip level of detail (LOD), where 0 is the maximum detail. This value must be greater than or equal
    /// to the <see cref="MinLod"/>. If both values are 0, mipmapping is effectively disabled.
    /// </summary>
    public float MaxLod;
    
    //public ColorF BorderColor;

    /// <summary>
    /// Create a <see cref="SamplerInfo"/>.
    /// </summary>
    /// <param name="minFilter">The minification filter.</param>
    /// <param name="magFilter">The magnification filter.</param>
    /// <param name="mipFilter">The mipmap filter.</param>
    /// <param name="addressU">How a U texture coordinate that is outside the 0-1 range should be resolved.</param>
    /// <param name="addressV">How a V texture coordinate that is outside the 0-1 range should be resolved.</param>
    /// <param name="addressW">How a W texture coordinate that is outside the 0-1 range should be resolved.</param>
    /// <param name="maxAnisotropy">The max anisotropy level. Set to 0 to disable.</param>
    /// <param name="minLod">The minimum mip level of detail (LOD), where 0 is the maximum detail.</param>
    /// <param name="maxLod">The maximum mip level of detail (LOD), where 0 is the maximum detail. This value must be
    /// greater than or equal to the <see cref="MinLod"/>. If both values are 0, mipmapping is effectively disabled.</param>
    public SamplerInfo(Filter minFilter, Filter magFilter, Filter mipFilter, AddressMode addressU, AddressMode addressV,
        AddressMode addressW, uint maxAnisotropy = 0, float minLod = 0, float maxLod = 1000/*, ColorF borderColor = default*/)
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

    /// <summary>
    /// Create a <see cref="SamplerInfo"/> with a filter and address mode.
    /// </summary>
    /// <param name="filter">The filter to use for the min, mag, and mip filters.</param>
    /// <param name="addressMode">The address mode to use for the U, V, and W texture coordinates that are outside the
    /// 0-1 range.</param>
    /// <param name="maxAnisotropy">The max anisotropy level. Set to 0 to disable.</param>
    /// <param name="minLod">The minimum mip level of detail (LOD), where 0 is the maximum detail.</param>
    /// <param name="maxLod">The maximum mip level of detail (LOD), where 0 is the maximum detail. This value must be
    /// greater than or equal to the <see cref="MinLod"/>. If both values are 0, mipmapping is effectively disabled.</param>
    public SamplerInfo(Filter filter, AddressMode addressMode, uint maxAnisotropy = 0, float minLod = 0,
        float maxLod = 1000/*, ColorF borderColor = default*/) : this(filter, filter, filter, addressMode, addressMode,
        addressMode, maxAnisotropy, minLod, maxLod/*, borderColor*/) { }

    /// <summary>
    /// A sampler with Point filtering and Wrap address mode.
    /// </summary>
    public static SamplerInfo PointWrap => new SamplerInfo(Filter.Point, AddressMode.Wrap);

    /// <summary>
    /// A sampler with Point filtering and ClampToEdge address mode.
    /// </summary>
    public static SamplerInfo PointClamp => new SamplerInfo(Filter.Point, AddressMode.ClampToEdge);

    /// <summary>
    /// A sampler with Linear filtering and Wrap address mode.
    /// </summary>
    public static SamplerInfo LinearWrap => new SamplerInfo(Filter.Linear, AddressMode.Wrap);

    /// <summary>
    /// A sampler with Linear filtering and ClampToEdge address mode.
    /// </summary>
    public static SamplerInfo LinearClamp => new SamplerInfo(Filter.Linear, AddressMode.ClampToEdge);

    /// <summary>
    /// A sampler with Anisotropic filtering. (Linear filter, Wrap address mode, and MaxAnisotropy as
    /// <see cref="MaxAnisotropicLevel"/>.
    /// </summary>
    public static SamplerInfo Anisotropic => new SamplerInfo(Filter.Linear, AddressMode.Wrap, MaxAnisotropicLevel);
}