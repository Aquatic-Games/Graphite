namespace Graphite.D3D11;

internal sealed class D3D11DescriptorSet : DescriptorSet
{
    public readonly D3D11DescriptorLayout Layout;

    public readonly Descriptor[] Descriptors;

    public D3D11DescriptorSet(DescriptorLayout layout, ReadOnlySpan<Descriptor> descriptors)
    {
        Layout = (D3D11DescriptorLayout) layout;

        Descriptors = descriptors.ToArray();
    }
    
    public override void Dispose() { }
}