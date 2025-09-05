namespace Graphite.D3D11;

public sealed class D3D11Backend : IBackend
{
    public static string Name => "DirectX 11";

    public static Backend Backend => Backend.D3D11;
    
    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        return new D3D11Instance(in info);
    }
}