namespace Graphite;

/// <summary>
/// Describes how a <see cref="DescriptorLayout"/> is created.
/// </summary>
public ref struct DescriptorLayoutInfo
{
    /// <summary>
    /// The <see cref="DescriptorBinding"/>s of the layout.
    /// </summary>
    public ReadOnlySpan<DescriptorBinding> Bindings;

    /// <summary>
    /// If true, this layout will be a push descriptor. This means that no <see cref="DescriptorSet"/> is created for
    /// the layout, and instead is pushed via the command buffer.
    /// </summary>
    /// <remarks>There can only be <b>one</b> push descriptor per pipeline.</remarks>
    public bool PushDescriptor;

    public DescriptorLayoutInfo(ReadOnlySpan<DescriptorBinding> bindings, bool pushDescriptor)
    {
        Bindings = bindings;
        PushDescriptor = pushDescriptor;
    }
}