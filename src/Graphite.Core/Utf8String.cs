using System.Runtime.InteropServices;
using System.Text;

namespace Graphite.Core;

public struct Utf8String : IDisposable
{
    private GCHandle _handle;

    public nint Handle => _handle.AddrOfPinnedObject();
    
    public Utf8String(string @string)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(@string);
        _handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    }

    public static implicit operator Utf8String(string str)
        => new Utf8String(str);
    
    public static unsafe implicit operator byte*(Utf8String str)
        => (byte*) str.Handle;

    public void Dispose()
    {
        _handle.Free();
    }
}