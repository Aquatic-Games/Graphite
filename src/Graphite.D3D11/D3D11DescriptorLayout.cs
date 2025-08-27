namespace Graphite.D3D11;

// In D3D11, the descriptor layout is nothing more than a dictionary of descriptor bindings, so we can remap them later.

internal sealed class D3D11DescriptorLayout : DescriptorLayout
{
    public readonly Dictionary<uint, DescriptorBinding> Layout;

    public readonly bool IsPushDescriptor;
    
    public D3D11DescriptorLayout(in DescriptorLayoutInfo info)
    {
        Layout = new Dictionary<uint, DescriptorBinding>(info.Bindings.Length);
        IsPushDescriptor = info.PushDescriptor;

        foreach (DescriptorBinding binding in info.Bindings)
        {
            Layout.Add(binding.Binding, binding);
        }
    }
    
    public override void Dispose() { }
}