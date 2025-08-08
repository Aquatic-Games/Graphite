using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Graphite.D3D11;

internal sealed unsafe class D3D11ShaderModule : ShaderModule
{
    public readonly void* Data;
    public readonly nuint DataLength;

    public D3D11ShaderModule(byte[] dxbc)
    {
        DataLength = (nuint) dxbc.Length;
        Data = NativeMemory.Alloc(DataLength);
        fixed (byte* pData = dxbc)
            Unsafe.CopyBlock(Data, pData, (uint) DataLength);
    }
    
    public override void Dispose() { }
}