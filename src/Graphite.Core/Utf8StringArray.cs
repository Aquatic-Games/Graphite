using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Graphite.Core;

public unsafe struct Utf8StringArray : IDisposable
{
    private readonly byte** _strings;

    public readonly uint Length;

    public nint Handle => (nint) _strings;

    public Utf8StringArray(params ReadOnlySpan<string> strings)
    {
        Length = (uint) strings.Length;

        // No point allocating if the length is 0.
        if (Length == 0)
            return;
        
        _strings = (byte**) NativeMemory.Alloc((nuint) (Length * sizeof(byte*)));

        for (int i = 0; i < Length; i++)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(strings[i]);
            
            uint length = (uint) stringBytes.Length;
            _strings[i] = (byte*) NativeMemory.Alloc(length + 1);
            
            fixed (byte* pString = stringBytes)
                Unsafe.CopyBlock(_strings[i], pString, length);

            _strings[i][length] = 0;
        }
    }

    public Utf8StringArray(List<string> strings) : this(CollectionsMarshal.AsSpan(strings)) { }

    public static implicit operator Utf8StringArray(string[] strings)
        => new Utf8StringArray(strings);
    
    public static implicit operator byte**(Utf8StringArray array)
        => array._strings;

    public void Dispose()
    {
        if (Length == 0)
            return;
        
        for (int i = 0; i < Length; i++)
            NativeMemory.Free(_strings[i]);
        
        NativeMemory.Free(_strings);
    }
}