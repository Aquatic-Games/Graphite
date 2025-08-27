namespace Graphite;

/// <summary>
/// A sampler is used to determine how a <see cref="Texture"/> is sampled in a shader.
/// </summary>
public abstract class Sampler : IDisposable
{
    /// <summary>
    /// Dispose of this <see cref="Sampler"/>.
    /// </summary>
    public abstract void Dispose();
}