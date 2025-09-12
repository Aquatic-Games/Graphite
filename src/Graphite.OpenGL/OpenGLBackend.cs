namespace Graphite.OpenGL;

public class OpenGLBackend : IBackend
{
    public const int MajorVersion = 4;

    public const int MinorVersion = 3;
    
    public static string Name => "OpenGL";

    public static Backend Backend => Backend.OpenGL;

    public static GLContext? Context;

    public Instance CreateInstance(ref readonly InstanceInfo info)
        => new GLInstance(in info);
}