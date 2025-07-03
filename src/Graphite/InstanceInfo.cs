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
    /// Create a new <see cref="InstanceInfo"/>.
    /// </summary>
    /// <param name="appName">The name of the application. This is used in some backends.</param>
    /// <param name="debug">Enable graphics debugging.</param>
    public InstanceInfo(string appName, bool debug = false)
    {
        AppName = appName;
        Debug = debug;
    }
}