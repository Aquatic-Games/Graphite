namespace Graphite.D3D11;

internal sealed class D3D11ShaderModule : ShaderModule
{
    public readonly byte[] Data;

    public D3D11ShaderModule(byte[] dxbc)
    {
        Data = dxbc;
    }
    
    public override void Dispose() { }
}