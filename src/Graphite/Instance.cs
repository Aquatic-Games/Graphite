using Graphite.D3D11;
using Graphite.Vulkan;

namespace Graphite;
/// <summary>
/// An <see cref="Instance"/> contains the core functions needed to create graphics objects.
/// </summary>
public abstract class Instance : IDisposable
{
    /// <summary>
    /// The API <see cref="Graphite.Backend"/> of this instance.
    /// </summary>
    public abstract Backend Backend { get; }
    
    /// <summary>
    /// Enumerate the supported <see cref="Adapter"/>s present on the system.
    /// </summary>
    /// <returns>The enumerated <see cref="Adapter"/>s.</returns>
    /// <remarks>If an empty array is returned, it means no adapters support the current backend. Try a different
    /// <see cref="Backend"/> instead.</remarks>
    public abstract Adapter[] EnumerateAdapters();

    /// <summary>
    /// Create a <see cref="Surface"/> for use in a <see cref="Swapchain"/>.
    /// </summary>
    /// <param name="info">The <see cref="SurfaceInfo"/> used to create the surface.</param>
    /// <returns>The created <see cref="Surface"/>.</returns>
    public abstract Surface CreateSurface(in SurfaceInfo info);

    /// <summary>
    /// Create a logical <see cref="Device"/>.
    /// </summary>
    /// <param name="surface">A surface to create the device with.</param>
    /// <param name="adapter">The <see cref="Adapter"/>, if any. If null is provided, the default adapter is used.</param>
    /// <returns>The created <see cref="Device"/>.</returns>
    public abstract Device CreateDevice(Surface surface, Adapter? adapter = null);
    
    /// <summary>
    /// Dispose of this <see cref="Instance"/>.
    /// </summary>
    public abstract void Dispose();

    /// <summary>
    /// Create an <see cref="Instance"/>. This will automatically pick the best <see cref="Backend"/> for the current
    /// system.
    /// </summary>
    /// <param name="info">The <see cref="InstanceInfo"/> used to create the instance.</param>
    /// <returns>The created <see cref="Instance"/>.</returns>
    public static Instance Create(in InstanceInfo info)
    {
        if (OperatingSystem.IsWindows())
            return new D3D11Instance(in info);
        
        return new VulkanInstance(in info);
    }
}