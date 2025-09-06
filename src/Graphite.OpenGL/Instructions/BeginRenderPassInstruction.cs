using Graphite.Core;

namespace Graphite.OpenGL.Instructions;

internal struct BeginRenderPassInstruction : IInstruction
{
    public GLTexture[] ColorAttachments;

    public ColorF ClearColor;

    public BeginRenderPassInstruction(GLTexture[] colorAttachments, ColorF clearColor)
    {
        ColorAttachments = colorAttachments;
        ClearColor = clearColor;
    }
}