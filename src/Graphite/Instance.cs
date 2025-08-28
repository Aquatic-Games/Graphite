using System.Diagnostics;
using Graphite.Core;

namespace Graphite;
/// <summary>
/// An <see cref="Instance"/> contains the core functions needed to create graphics objects.
/// </summary>
public abstract class Instance : IDisposable
{
    /// <summary>
    /// The name of the backend.
    /// </summary>
    public abstract string BackendName { get; }
    
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

    private static Dictionary<string, IBackendBase> _backends;

    static Instance()
    {
        _backends = [];
    }

    public static void RegisterBackend<T>() where T : IBackend, new()
    {
        _backends.Add(T.Name, new T());
    }

    /// <summary>
    /// Create an <see cref="Instance"/>. This will automatically pick the best <see cref="Graphite.Backend"/> for the current
    /// system.
    /// </summary>
    /// <param name="info">The <see cref="InstanceInfo"/> used to create the instance.</param>
    /// <returns>The created <see cref="Instance"/>.</returns>
    public static Instance Create(in InstanceInfo info)
    {
        Debug.Assert(_backends.Count > 0, "There must be at least 1 backend registered.");
        GraphiteLog.Log($"Registered backends: [{string.Join(", ", _backends.Keys)}]");

        foreach ((string name, IBackendBase backend) in _backends)
        {
            GraphiteLog.Log(GraphiteLog.Severity.Error, GraphiteLog.Type.General, $"Creating backend: {name}.");
            Instance instance;
            
            try
            {
                instance = backend.CreateInstance(in info);
            }
            catch (Exception e)
            {
                GraphiteLog.Log(GraphiteLog.Severity.Warning, GraphiteLog.Type.Other, $"Failed to create backend: {e}");
                continue;
            }
            
            return instance;
        }

        throw new PlatformNotSupportedException("None of the registered backends are supported on this system.");
    }
}