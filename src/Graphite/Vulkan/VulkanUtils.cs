using Graphite.Exceptions;
using Silk.NET.Vulkan;

namespace Graphite.Vulkan;

internal static class VulkanUtils
{
    public static void Check(this Result result, string operation)
    {
        if (result != Result.Success)
            throw new OperationFailedException($"Vulkan operation '{operation}' failed: {result}");
    }
}