namespace Graphite;

public struct InstanceInfo
{
    public string AppName;

    public bool Debug;

    public InstanceInfo(string appName, bool debug = false)
    {
        AppName = appName;
        Debug = debug;
    }
}