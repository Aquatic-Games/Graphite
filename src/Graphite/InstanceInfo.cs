namespace Graphite;

/// <summary>
/// Describes how an <see cref="Instance"/> should be created.
/// </summary>
public struct InstanceInfo
{
    /// <summary>
    /// The name of the application. This is used in some backends.
    /// </summary>
    public string AppName;

    /// <summary>
    /// Enable graphics debugging.
    /// </summary>
    public bool Debug;

    /// <summary>
    /// The GetProcAddress function for the OpenGL backend. This does not need to be provided if the OpenGL backend
    /// will not be used.
    /// </summary>
    public Func<string, nint>? GLGetProcAddressFunc;

    /// <summary>
    /// Create a new <see cref="InstanceInfo"/>.
    /// </summary>
    /// <param name="appName">The name of the application. This is used in some backends.</param>
    /// <param name="debug">Enable graphics debugging.</param>
    /// <param name="glGetProcAddressFunc">The GetProcAddress function for the OpenGL backend.</param>
    public InstanceInfo(string appName, bool debug = false, Func<string, nint>? glGetProcAddressFunc = null)
    {
        AppName = appName;
        Debug = debug;
        GLGetProcAddressFunc = glGetProcAddressFunc;
    }
}