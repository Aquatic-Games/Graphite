namespace Graphite;

/// <summary>
/// Remap a descriptor to a slot.
/// </summary>
public struct DescriptorMapping
{
    /// <summary>
    /// The descriptor set index.
    /// </summary>
    public uint Set;

    /// <summary>
    /// The binding index.
    /// </summary>
    public uint Binding;

    /// <summary>
    /// The slot to map to.
    /// </summary>
    public uint Slot;

    /// <summary>
    /// Create a new <see cref="DescriptorMapping"/>
    /// </summary>
    /// <param name="set">The descriptor set index.</param>
    /// <param name="binding">The binding index.</param>
    /// <param name="slot">The slot to map to.</param>
    public DescriptorMapping(uint set, uint binding, uint slot)
    {
        Set = set;
        Binding = binding;
        Slot = slot;
    }
}