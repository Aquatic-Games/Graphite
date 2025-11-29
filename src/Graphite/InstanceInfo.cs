namespace Graphite;

public record struct InstanceInfo
{
    public string AppName;

    public bool Debug;

    public InstanceInfo(string appName, bool debug)
    {
        AppName = appName;
        Debug = debug;
    }
}