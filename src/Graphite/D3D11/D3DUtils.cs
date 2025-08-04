using System.Diagnostics.CodeAnalysis;
using Graphite.Exceptions;
using TerraFX.Interop.Windows;

namespace Graphite.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal static class D3DUtils
{
    public static void Check(this HRESULT result, string operation)
    {
        if (result.FAILED)
            throw new OperationFailedException($"D3D11 operation '{operation}' failed with HRESULT: {result.Value}");
    }
}