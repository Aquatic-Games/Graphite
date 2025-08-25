namespace Graphite.D3D11;

// In D3D11, the descriptor layout is nothing more than a dictionary of descriptor bindings, so we can remap them later.

internal sealed class D3D11DescriptorLayout : DescriptorLayout
{
    public readonly Dictionary<uint, DescriptorBinding> Layout;
    
    public D3D11DescriptorLayout(ReadOnlySpan<DescriptorBinding> bindings)
    {
        Layout = new Dictionary<uint, DescriptorBinding>(bindings.Length);

        foreach (DescriptorBinding binding in bindings)
        {
            Layout.Add(binding.Binding, binding);
        }
    }
    
    public override void Dispose() { }
}