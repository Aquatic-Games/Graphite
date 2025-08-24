using Graphite.OpenGL;

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
    /// The OpenGL context info, if any. You do not need to provide this if you will not be using the OpenGL backend. 
    /// </summary>
    public GLContext? GLContext;

    /// <summary>
    /// Create a new <see cref="InstanceInfo"/>.
    /// </summary>
    /// <param name="appName">The name of the application. This is used in some backends.</param>
    /// <param name="debug">Enable graphics debugging.</param>
    /// <param name="context">The OpenGL context info, if any.</param>
    public InstanceInfo(string appName, bool debug = false, GLContext? context = null)
    {
        AppName = appName;
        Debug = debug;
        GLContext = context;
    }
}