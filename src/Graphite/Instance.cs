using System.Runtime.CompilerServices;

namespace Graphite;

public abstract class Instance : IDisposable
{
    /// <summary>
    /// Subscribe to various debug log messages. This can be useful for debugging.
    /// </summary>
    public static event OnLogMessage LogMessage;
    
    /// <summary>
    /// Returns true if this instance is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Enumerates the supported <see cref="Adapter"/>s that can be used to create a <see cref="Device"/>.
    /// </summary>
    /// <returns>The supported adapters.</returns>
    public abstract Adapter[] EnumerateAdapters();

    /// <summary>
    /// Create a <see cref="Device"/>.
    /// </summary>
    /// <param name="surface">The <see cref="Surface"/> to use when creating the device.</param>
    /// <param name="adapter">The <see cref="Adapter"/> to use, if any. If none is provided, then the default adapter is used.</param>
    /// <returns>The created <see cref="Device"/>.</returns>
    public abstract Device CreateDevice(Surface surface, Adapter? adapter = null);

    /// <summary>
    /// Create a <see cref="Surface"/> from the given <see cref="SurfaceInfo"/>.
    /// </summary>
    /// <param name="info">The <see cref="SurfaceInfo"/> that describes how the surface is created.</param>
    /// <returns>The created <see cref="Surface"/>.</returns>
    public abstract Surface CreateSurface(in SurfaceInfo info);

    /// <summary>
    /// Dispose of this instance.
    /// </summary>
    public abstract void Dispose();

    static Instance()
    {
        LogMessage = delegate { };
    }
    
    public static void Log(LogSeverity severity, string message, [CallerLineNumber] int line = 0,
        [CallerFilePath] string path = "")
    {
        LogMessage(severity, message, line, path);
    }

    public static void Log(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string path = "")
        => Log(LogSeverity.Trace, message, line, path);

    public enum LogSeverity
    {
        Trace,
        Info,
        Warning,
        Error
    }

    public delegate void OnLogMessage(LogSeverity severity, string message, int line, string path);
}